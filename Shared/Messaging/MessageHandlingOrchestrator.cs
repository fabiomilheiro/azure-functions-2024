using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Azf.Shared.Messaging;

public interface IMessageHandlingOrchestrator
{
    Task HandleAsync(string message);
}

public class MessageHandlingOrchestrator : IMessageHandlingOrchestrator
{
    private readonly IServiceProvider serviceProvider;
    private readonly IJsonService jsonService;
    private readonly ILogger<MessageHandlingOrchestrator> logger;

    public MessageHandlingOrchestrator(
        IServiceProvider serviceProvider,
        IJsonService jsonService,
        ILogger<MessageHandlingOrchestrator> logger)
    {
        this.serviceProvider = serviceProvider;
        this.jsonService = jsonService;
        this.logger = logger;
    }

    public async Task HandleAsync(string message)
    {
        try
        {
            var queueMessageTriggerData = this.jsonService.Deserialize<QueueMessageTriggerData>(message);

            using var scope = this.serviceProvider.CreateScope();

            var asyncMessageHandlerFactory = scope.ServiceProvider.GetRequiredService<IAsyncMessageHandlerFactory>();
            var asyncMessageHandlerCreateResult =
                asyncMessageHandlerFactory.Create(queueMessageTriggerData.TypeName);

            var asyncMessage = this.DeserializeAsyncMessage(message, asyncMessageHandlerCreateResult);

            await asyncMessageHandlerCreateResult.Handler.HandleAsync(asyncMessage);
        }
        catch (Exception e)
        {
            this.logger.LogError(e, $"Could not handle message correctly: {message}");
            throw;
        }
    }

    private AsyncMessage DeserializeAsyncMessage(
        string message,
        AsyncMessageHandlerCreateResult asyncMessageHandlerCreateResult)
    {
        return (AsyncMessage)this.jsonService.Deserialize(message, asyncMessageHandlerCreateResult.MessageType);
    }
}