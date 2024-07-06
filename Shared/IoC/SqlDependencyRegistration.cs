using Backend.App.Data.Outbox;
using Backend.App.Data.Sql.Context;
using Backend.App.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Azf.Shared.IoC;

public class SqlDependencyRegistration : IDependencyRegistration
{
    public void Execute(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>((serviceProvider, builder) =>
        {
            var appSettings = serviceProvider.GetRequiredService<AppSettings>();
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
        services.AddScoped<IOutboxMessageService, OutboxMessageService>();
    }
}