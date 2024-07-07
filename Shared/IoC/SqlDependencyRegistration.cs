using Azf.Shared.Configuration;
using Azf.Shared.Sql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Azf.Shared.IoC;

public class SqlDependencyRegistration : IDependencyRegistration
{
    public void Execute(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SqlDbContext>((serviceProvider, builder) =>
        {
            var appSettings = serviceProvider.GetRequiredService<SharedSettings>();
            builder
                .EnableSensitiveDataLogging(appSettings.EnableSensitiveDataLogging)
                .UseLazyLoadingProxies()
                .UseSqlServer(appSettings.SqlConnectionString,
                    optionsBuilder =>
                    {
                        optionsBuilder.EnableRetryOnFailure(
                            10,
                            TimeSpan.FromSeconds(5),
                            null);
                    });
        });

        //services.AddEntityFrameworkProxies();
        services.AddSqlDependencies();
    }
}