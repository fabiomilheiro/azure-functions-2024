using Azf.Shared.Configuration;
using Azf.Shared.IoC;
using Azf.Shared.Json;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Azf.Shared.Sql;

public abstract class DesignTimeSqlDbContextFactory<TDbContext> : IDesignTimeDbContextFactory<TDbContext>
   where TDbContext : SqlDbContext
{
    public TDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            //.AddUserSecrets<SharedSettings>()
            .AddConfigurations(
                new AddConfigurationsRequest
                {
                    ApplicationRootPath = AppDomain.CurrentDomain.BaseDirectory,
                    EnvironmentName = AppEnvironment.Development.ToString(),
                })
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    { "Environment", AppEnvironment.Development.ToString() },
                })
            .Build();

        var serviceProvider = new ServiceCollection()
                              .AddSingleton<IConfiguration>(configuration)
                              .AddCommonServices()
                              .AddSqlDbContext<TDbContext>()
                              .BuildServiceProvider();

        return serviceProvider.GetRequiredService<TDbContext>();
    }
}