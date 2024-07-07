using Azf.Shared.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Azf.Shared.IoC;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        var dependencyRegistrations = TypeRepository
                                      .GetConcreteSubTypesOf<IDependencyRegistration>()
                                      .Select(x => (IDependencyRegistration)Activator.CreateInstance(x)!)
                                      .ToArray();

        foreach (var dependencyRegistration in dependencyRegistrations)
        {
            dependencyRegistration.Execute(services, configuration);
        }

        return services;
    }
}