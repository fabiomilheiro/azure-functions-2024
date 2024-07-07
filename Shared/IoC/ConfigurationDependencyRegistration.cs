﻿using Azf.Shared.Configuration;
using Azf.Shared.Time;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Azf.Shared.IoC;

public class ConfigurationDependencyRegistration : IDependencyRegistration
{
    public void Execute(IServiceCollection services, DependencyRegistrationContext context)
    {
        services.AddOptions<SharedSettings>().Configure<IConfiguration>((settings, configuration) =>
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

            configuration.Bind(settings);

            ArgumentException.ThrowIfNullOrWhiteSpace(
                settings.SqlConnectionString,
                nameof(SharedSettings.SqlConnectionString));

            ArgumentException.ThrowIfNullOrWhiteSpace(
                settings.ServiceBusConnectionString,
                nameof(SharedSettings.ServiceBusConnectionString));

            ArgumentException.ThrowIfNullOrWhiteSpace(
                settings.BunnyStorageApiKey,
                nameof(SharedSettings.BunnyStorageApiKey));

            ArgumentException.ThrowIfNullOrWhiteSpace(
                settings.BunnyStorageApiBaseUrl,
                nameof(SharedSettings.BunnyStorageApiBaseUrl));


            settings.EnableSensitiveDataLogging = parsedEnvironment != AppEnvironment.Production;
        });
        services.AddTransient(serviceProvider =>
            serviceProvider.GetRequiredService<IOptions<SharedSettings>>().Value);

        services.AddSingleton<IClock, Clock>();
    }
}