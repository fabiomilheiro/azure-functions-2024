using Azf.Shared.Configuration;
using Azf.Shared.Json;
using Azure.Messaging.ServiceBus;

namespace Azf.Shared.Messaging;

public interface IQueueClient
{
    Task SendAsync(AsyncMessage message);

    Task SendAsync(IEnumerable<AsyncMessage> messages);
}

public class QueueClient : IQueueClient
{
    private readonly IJsonService jsonService;
    private readonly ServiceBusClient serviceBus;
    private readonly AppSettings settings;

    public QueueClient(ServiceBusClient serviceBus, AppSettings settings, IJsonService jsonService)
    {
        this.serviceBus = serviceBus;
        this.settings = settings;
        this.jsonService = jsonService;
    }

    public Task SendAsync(AsyncMessage message)
    {
        return this.SendAsync(new[] { message });
    }

    public async Task SendAsync(IEnumerable<AsyncMessage> messages)
    {
        var serviceBusMessages =
            messages
                .Select(message =>
                    new ServiceBusMessage(this.jsonService.Serialize(message)))
                .ToArray();

        await this.serviceBus
                  .CreateSender(this.settings.ServiceBusQueueName)
                  .SendMessagesAsync(serviceBusMessages);
    }
}