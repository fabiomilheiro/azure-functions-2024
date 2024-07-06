namespace Azf.Shared.Configuration;

public class AppSettings
{
    public AppEnvironment Environment { get; set; }

    public string JwtSecretKey { get; set; }

    public string JwtAuthority { get; set; }

    public string JwtIssuer { get; set; }

    public string JwtAudience { get; set; }

    public string SqlConnectionString { get; set; }

    public string ServiceBusConnectionString { get; set; }

    public string ServiceBusQueueName { get; set; }

    public string BunnyStorageApiKey { get; set; }

    public string BunnyStorageApiBaseUrl { get; set; }

    public bool EnableSensitiveDataLogging { get; set; }
}