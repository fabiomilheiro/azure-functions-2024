using Backend.App.Infrastructure.Configuration;
using IEXSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Azf.Shared.IoC;

public class IexDependencyRegistration : IDependencyRegistration
{
    public void Execute(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(serviceProvider =>
        {
            var appSettings = serviceProvider.GetRequiredService<AppSettings>();

            if (appSettings.IexUseSandbox)
                return new IEXCloudClient(appSettings.IexSandboxPublishableToken, appSettings.IexSandboxSecretToken,
                    false, true);

            return new IEXCloudClient(appSettings.IexProductionPublishableToken, appSettings.IexProductionSecretToken,
                false, false);
        });
    }
}