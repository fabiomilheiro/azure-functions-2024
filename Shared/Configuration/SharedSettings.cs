namespace Azf.Shared.Configuration;

public class SharedSettings
{
    public AppEnvironment Environment { get; set; }

    public required string JwtSecretKey { get; set; }

    public required string JwtAuthority { get; set; }

    public required string JwtIssuer { get; set; }

    public required string JwtAudience { get; set; }

    public required string SqlConnectionString { get; set; }

    public required string ServiceBusConnectionString { get; set; }

    //public required string ServiceBusQueueName { get; set; }

    public required string BunnyStorageApiKey { get; set; }

    public required string BunnyStorageApiBaseUrl { get; set; }

    public required bool EnableSensitiveDataLogging { get; set; }
}