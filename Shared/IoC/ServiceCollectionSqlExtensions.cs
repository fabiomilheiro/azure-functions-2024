using Azf.Shared.Configuration;
using Azf.Shared.Sql;
using Azf.Shared.Sql.ChangeHandling;
using Azf.Shared.Sql.OnModelCreating;
using Azf.Shared.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Azf.Shared.IoC;

public static class ServiceCollectionSqlExtensions
{
    public static void AddSqlDbContext<TDbContext>(this IServiceCollection services)
        where TDbContext : SqlDbContext
    {
        services.AddDbContext<TDbContext>((serviceProvider, builder) =>
        {
            var appSettings = serviceProvider.GetRequiredService<SharedSettings>();
            builder
                .EnableSensitiveDataLogging(appSettings.EnableSensitiveDataLogging)
                .UseLazyLoadingProxies()
                .UseSqlServer(appSettings.SqlConnectionString,
                    optionsBuilder =>
                    {
                        optionsBuilder.EnableRetryOnFailure(
                            10,
                            TimeSpan.FromSeconds(5),
                            null);
                    });
        });

        AddOnModelCreatingServices(services);
        AddEntityChangeHandlingServices(services);
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