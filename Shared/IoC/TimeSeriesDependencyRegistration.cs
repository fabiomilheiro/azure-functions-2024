using System.Text.Json.Serialization;
using Backend.App.Data.TimeSeries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Azf.Shared.IoC;

public class TimeSeriesDependencyRegistration : IDependencyRegistration
{
    public void Execute(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITimeSeries, TimeSeries>();
    }
}