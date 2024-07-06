using System.Reflection;
using Azure.Identity;
using Microsoft.Extensions.Configuration;

namespace Azf.Shared.Configuration;

public class AddConfigurationsRequest
{
    public required string EnvironmentName { get; set; }

    public required string ApplicationRootPath { get; set; }
}

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddConfigurations(
        this IConfigurationBuilder configurationBuilder,
        AddConfigurationsRequest request)
    {
        configurationBuilder
            .AddJsonFile(Path.Combine(request.ApplicationRootPath, "settings.json"))
            .AddJsonFile(Path.Combine(request.ApplicationRootPath, $"settings.{request.EnvironmentName}.json"));



        if (request.EnvironmentName == AppEnvironment.Development.ToString())
        {
            configurationBuilder.AddUserSecrets(Assembly.GetAssembly(typeof(AppSettings))!);
        }
        else
        {
            configurationBuilder.AddAzureKeyVault(new Uri(""), new DefaultAzureCredential());
        }


        return configurationBuilder;
    }
}