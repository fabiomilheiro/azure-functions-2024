using Azf.Shared.Configuration;
using Azf.Shared.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Azf.Shared.IoC;

public class JsonDependencyRegistration : IDependencyRegistration
{
    public void Execute(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IJsonService, JsonService>();
        services.AddSingleton(
            serviceProvider => JsonSerializerOptionsFactory.GetDefault(
                serviceProvider.GetRequiredService<SharedSettings>()
            ));
    }
}