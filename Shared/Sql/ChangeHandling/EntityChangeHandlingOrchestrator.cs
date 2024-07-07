using Azf.Shared.Types;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;

namespace Azf.Shared.Sql.ChangeHandling;

public interface IEntityChangeHandlingOrchestrator
{
    void Handle(SqlDbContext db);
}

public class EntityChangeHandlingOrchestrator : IEntityChangeHandlingOrchestrator
{
    private static readonly Dictionary<Type, Type> EntityToHandlerMappings;

    public static readonly Type[] EntityChangeHandlerTypes;

    static EntityChangeHandlingOrchestrator()
    {
        EntityChangeHandlerTypes = GetEntityChangeHandlerTypes();
        EntityToHandlerMappings = GetEntityToHandlerMappings();
    }
    private readonly IServiceProvider serviceProvider;

    //private readonly IEntityChangeHandler[] entityChangeHandlers;

    //public EntityChangeHandlingOrchestrator(IEnumerable<IEntityChangeHandler> entityChangeHandlers)
    //{
    //    this.entityChangeHandlers = entityChangeHandlers.ToArray();
    //}

    public EntityChangeHandlingOrchestrator(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public void Handle(SqlDbContext db)
    {
        var entries = db.ChangeTracker.Entries().ToArray();

        var entityChangeHandlers = GetEntityChangeHandlersByEntries(entries);

        foreach (var entityChangeHandler in entityChangeHandlers)
        {
            var matchingEntries = entries.Where(e => e.Entity.GetType().IsAssignableTo(entityChangeHandler.EntityType));

            foreach (var matchingEntry in matchingEntries)
            {
                entityChangeHandler.Handle(matchingEntry);
            }
        }
    }

    private static Dictionary<Type, Type> GetEntityToHandlerMappings()
    {
        var mappings = new Dictionary<Type, Type>();

        foreach (var entityChangeHandlerType in EntityChangeHandlerTypes)
        {
            var entityType = entityChangeHandlerType.BaseType!.GetGenericArguments().Single();

            mappings.Add(entityType, entityChangeHandlerType);
        }

        return mappings;
    }

    private static Type[] GetEntityChangeHandlerTypes()
    {
        return TypeRepository
               .GetTypes()
               .Where(x => x.IsConcreteSubTypeOf<IEntityChangeHandler>())
               .ToArray();
    }

    private IEntityChangeHandler[] GetEntityChangeHandlersByEntries(EntityEntry[] entries)
    {
        var entityChangeHandlerTypes = GetEntityChangeHandlerTypesByEntries(entries);

        return entityChangeHandlerTypes
               .Select(type => (IEntityChangeHandler)serviceProvider.GetRequiredService(type))
               .ToArray();
    }

    private static Type[] GetEntityChangeHandlerTypesByEntries(EntityEntry[] entries)
    {
        return entries
               .GroupBy(e => e.Entity.GetType())
               .Select(g => g.Key)
               .SelectMany(GetEntityChangeHandlerTypesByEntityType)
               .Distinct()
               .ToArray();

        // TODO: Memoize.
        static Type[] GetEntityChangeHandlerTypesByEntityType(Type entityType)
        {
            var assignableEntityTypes = GetAssignableEntityTypes(entityType);

            return assignableEntityTypes
                   .Where(t => EntityToHandlerMappings.ContainsKey(t))
                   .Select(t => EntityToHandlerMappings[t])
                   .ToArray();
        }

        static Type[] GetAssignableEntityTypes(Type entityType)
        {
            return new[] { entityType }
                   .Union(entityType.GetInterfaces())
                   .Union(GetBaseTypes(entityType))
                   .Distinct()
                   .ToArray();
        }
    }

    private static Type[] GetBaseTypes(Type type)
    {
        var baseTypes = new List<Type>();
        var baseType = type.BaseType;

        while (baseType != typeof(object))
        {
            baseTypes.Add(baseType);
            baseType = baseType!.BaseType;
        }

        return baseTypes.ToArray();
    }
}