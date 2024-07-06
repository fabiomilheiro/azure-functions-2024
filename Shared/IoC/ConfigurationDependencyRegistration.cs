using Azf.Shared.Configuration;
using Azf.Shared.Time;
using Backend.App.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Azf.Shared.IoC;

public class ConfigurationDependencyRegistration : IDependencyRegistration
{
    public void Execute(IServiceCollection services, IConfiguration configuration1)
    {
        services.AddOptions<AppSettings>().Configure<IConfiguration>((settings, configuration) =>
        {
            var environmentValue = configuration["ASPNETCORE_ENVIRONMENT"];
            if (!Enum.TryParse(environmentValue, true, out AppEnvironment parsedEnvironment))
            {
                throw new Exception(
                    $"Environment '{environmentValue}' not valid.{Environment.NewLine}" +
                    $"Please select one of the appropriate values or add it to the list:" +
                    $"{Environment.NewLine}[{string.Join(", ", typeof(AppEnvironment).GetEnumValues().Cast<AppEnvironment>())}]");
            }

            settings.Environment = parsedEnvironment;
            settings.SqlConnectionString = configuration["SqlConnectionString"];
            settings.ServiceBusConnectionString = configuration["ServiceBusConnectionString"];
            settings.ServiceBusQueueName = configuration["ServiceBusQueueName"];
            //settings.SearchBaseUrl = configuration["SearchBaseUrl"];
            //settings.SearchAdminApiKey = configuration["SearchAdminApiKey"];
            settings.BunnyStorageApiKey = configuration["BunnyStorageApiKey"];
            settings.BunnyStorageApiBaseUrl = configuration["BunnyStorageApiBaseUrl"];
            bool.TryParse(configuration["EnableSensitiveDataLogging"], out var enableSensitiveDataLogging);
            settings.EnableSensitiveDataLogging = parsedEnvironment != AppEnvironment.Production;
        });
        services.AddTransient(serviceProvider =>
            serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value);

        services.AddSingleton<IClock, Clock>();
    }
}