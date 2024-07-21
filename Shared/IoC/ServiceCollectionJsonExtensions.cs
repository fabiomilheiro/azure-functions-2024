using Azf.Shared.Configuration;
using Azf.Shared.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Azf.Shared.IoC;

public static class ServiceCollectionJsonExtensions
{
    public static IServiceCollection AddJson(this IServiceCollection services)
    {
        services.AddSingleton<IJsonService, JsonService>();
        services.AddSingleton(
            serviceProvider => JsonSerializerOptionsFactory.GetDefault(
                serviceProvider.GetRequiredService<SharedSettings>()
            ));

        return services;
    }
}