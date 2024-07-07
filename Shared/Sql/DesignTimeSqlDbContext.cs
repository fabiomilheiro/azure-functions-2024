using Azf.Shared.Configuration;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azf.Shared.Sql;

public class DesignTimeAppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
                            .AddConfigurations()
                            .AddInMemoryCollection(
                                new Dictionary<string, string>
                                {
                                    { "ASPNETCORE_ENVIRONMENT", AppEnvironment.Local.ToString() },
                                })
                            .Build();

        var serviceProvider = new ServiceCollection()
                              .AddServices(configuration)
                              .AddSingleton<IConfiguration>(configuration)
                              .BuildServiceProvider();

        return serviceProvider.GetRequiredService<AppDbContext>();
    }
}