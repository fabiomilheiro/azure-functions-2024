using Azf.Shared.Configuration;
using Azf.Shared.Messaging;
using Azf.Shared.Sql.Outbox;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;

namespace Azf.Shared.IoC;

public static class ServiceCollectionMessagingExtensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services)
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
        services.AddScoped<IOutboxRelayer<QueueMessage>, OutboxRelayerBase<QueueMessage>>();
        services.AddScoped<IOutboxRelayer<TopicMessage>, OutboxRelayerBase<TopicMessage>>();

        return services;
    }
}