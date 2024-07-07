using Azf.Shared.Configuration;
using Azf.Shared.Json;
using Azure.Messaging.ServiceBus;

namespace Azf.Shared.Messaging;

public interface IQueueClient
{
    Task SendAsync(string queueName, AsyncMessage message);

    Task SendAsync(string queueName, IEnumerable<AsyncMessage> messages);
}

public class QueueClient : IQueueClient
{
    private readonly IJsonService jsonService;
    private readonly ServiceBusClient serviceBus;
    private readonly SharedSettings settings;

    public QueueClient(ServiceBusClient serviceBus, SharedSettings settings, IJsonService jsonService)
    {
        this.serviceBus = serviceBus;
        this.settings = settings;
        this.jsonService = jsonService;
    }

    public Task SendAsync(string queueName, AsyncMessage message)
    {
        return this.SendAsync(queueName, [message]);
    }

    public async Task SendAsync(string queueName, IEnumerable<AsyncMessage> messages)
    {
        var serviceBusMessages =
            messages
                .Select(message =>
                    new ServiceBusMessage(this.jsonService.Serialize(message)))
                .ToArray();

        await this.serviceBus
                  .CreateSender(queueName.ToString())
                  .SendMessagesAsync(serviceBusMessages);
    }
}