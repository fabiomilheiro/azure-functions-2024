using Azf.Shared.Sql;
using Azf.Shared.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Azf.Shared.IoC;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services)
    {
        services.AddConfiguration();
        services.AddJson();
        services.AddMessaging();
        
        return services;
    }
}