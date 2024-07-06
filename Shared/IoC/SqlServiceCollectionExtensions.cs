using Backend.App.Data.Sql.ChangeHandling;
using Backend.App.Data.Sql.OnModelCreating;
using Backend.App.Infrastructure.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Azf.Shared.IoC;

public static class SqlServiceCollectionExtensions
{
    public static IServiceCollection AddSqlDependencies(this IServiceCollection services)
    {
        AddOnModelCreatingServices(services);
        AddEntityChangeHandlingServices(services);
        return services;
    }

    private static void AddOnModelCreatingServices(IServiceCollection services)
    {
        services.AddSingleton<IOnModelCreatingOrchestrator, OnModelCreatingOrchestrator>();

        var onModelCreatingHandlerTypes =
            TypeRepository
                .GetTypes()
                .Where(x => x.IsConcreteSubTypeOf<IOnModelCreatingHandler>())
                .ToArray();

        foreach (var onModelCreatingHandlerType in onModelCreatingHandlerTypes)
        {
            services.AddSingleton(typeof(IOnModelCreatingHandler), onModelCreatingHandlerType);
        }
    }

    private static void AddEntityChangeHandlingServices(IServiceCollection services)
    {
        services.AddScoped<IEntityChangeHandlingOrchestrator, EntityChangeHandlingOrchestrator>();

        foreach (var entityChangeHandlerType in EntityChangeHandlingOrchestrator.EntityChangeHandlerTypes)
        {
            if (entityChangeHandlerType.BaseType!.GetGenericTypeDefinition() != typeof(EntityChangeHandlerBase<>))
            {
                throw new Exception(
                    $"Entity change handler '{entityChangeHandlerType}' must implement {typeof(EntityChangeHandlerBase<>)}");
            }

            services.AddScoped(entityChangeHandlerType);
        }
    }
}