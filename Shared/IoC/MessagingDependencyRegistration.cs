using Azf.Shared.Configuration;
using Azf.Shared.Messaging;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Azf.Shared.IoC;

public class MessagingDependencyRegistration : IDependencyRegistration
{
    public void Execute(IServiceCollection services, DependencyRegistrationContext context)
    {
        services.AddScoped<IMessageHandlingOrchestrator, MessageHandlingOrchestrator>();
        services.AddScoped<IAsyncMessageHandlerFactory, AsyncMessageHandlerFactory>();

        foreach (var mapping in AsyncMessageMappings.ByMessageTypeName)
        {
            services.AddScoped(mapping.Value.HandlerType);
        }

        services.AddSingleton((serviceProvider) =>
        {
            var settings = serviceProvider.GetRequiredService<SharedSettings>();
            return new ServiceBusClient(settings.ServiceBusConnectionString);
        });

        services.AddScoped<IQueueClient, QueueClient>();
        services.AddScoped<IOutboxRelayer, OutboxRelayer>();
    }
}