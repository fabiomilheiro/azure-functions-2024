using Azf.Shared;
using Azf.Shared.Configuration;
using Azf.UserService;
using Azf.UserService.Helpers;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Azf.UserService
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            var context = builder.GetContext();

            builder.ConfigurationBuilder.AddConfigurations(
                new AddConfigurationsRequest
                {
                    ApplicationRootPath = context.ApplicationRootPath,
                    EnvironmentName = context.EnvironmentName,
                });
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IExampleService, DefaultExampleService>();
            builder.Services.AddOptions<UserServiceSettings>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.Bind(settings);
            });
        }
    }
}
