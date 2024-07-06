using Azure.Messaging.ServiceBus;
using Backend.App.Infrastructure.Configuration;
using Backend.App.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Azf.Shared.IoC;

public class MessagingDependencyRegistration : IDependencyRegistration
{
    public void Execute(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IMessageHandlingOrchestrator, MessageHandlingOrchestrator>();
        services.AddScoped<IAsyncMessageHandlerFactory, AsyncMessageHandlerFactory>();

        foreach (var mapping in AsyncMessageMappings.ByMessageTypeName)
        {
            services.AddScoped(mapping.Value.HandlerType);
        }

        services.AddSingleton((serviceProvider) =>
        {
            var settings = serviceProvider.GetRequiredService<AppSettings>();
            return new ServiceBusClient(settings.ServiceBusConnectionString);
        });

        services.AddScoped<IQueueClient, QueueClient>();
        services.AddScoped<IOutboxRelayer, OutboxRelayer>();
    }
}