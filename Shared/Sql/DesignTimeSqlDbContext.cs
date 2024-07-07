using Azf.Shared.Configuration;
using Azf.Shared.IoC;
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
            .AddConfigurations(
            new AddConfigurationsRequest
            {
                ApplicationRootPath = "",
                EnvironmentName = AppEnvironment.Development.ToString(),
            })
                            .AddInMemoryCollection(
                                new Dictionary<string, string?>
                                {
                                    { "ASPNETCORE_ENVIRONMENT", AppEnvironment.Development.ToString() },
                                })
                            .Build();

        var serviceProvider = new ServiceCollection()
                              .AddServices(
                                new DependencyRegistrationContext
                                {
                                    Configuration = configuration,
                                })
                              .AddSingleton<IConfiguration>(configuration)
                              .BuildServiceProvider();

        return serviceProvider.GetRequiredService<TDbContext>();
    }
}