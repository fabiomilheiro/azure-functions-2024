using Backend.App.Infrastructure.Firebase;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Azf.Shared.IoC;

public class FirebaseDependencyRegistration : IDependencyRegistration
{
    public void Execute(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IFirebaseAppFactory, FirebaseAppFactory>();
        services.AddSingleton(serviceProvider =>
            serviceProvider.GetRequiredService<IFirebaseAppFactory>().CreateDefault());
        services.AddSingleton(serviceProvider =>
            FirebaseAuth.GetAuth(serviceProvider.GetRequiredService<FirebaseApp>()));
    }
}