using Microsoft.Extensions.Logging;

namespace Azf.Shared.Messaging.CommonHandlers;

public class RelayOutboxMessagesAsyncMessage
    : AsyncMessage
{
}

// TODO: Do I still need this if we use the change tracker?
public class RelayOutboxMessagesAsyncMessageHandler : AsyncMessageHandlerBase<RelayOutboxMessagesAsyncMessage>
{
    private readonly IOutboxRelayer outboxRelayer;
    private readonly ILogger<RelayOutboxMessagesAsyncMessageHandler> logger;

    public RelayOutboxMessagesAsyncMessageHandler(
        IOutboxRelayer outboxRelayer,
        ILogger<RelayOutboxMessagesAsyncMessageHandler> logger)
    {
        this.outboxRelayer = outboxRelayer;
        this.logger = logger;
    }

    protected override async Task HandleAsync(RelayOutboxMessagesAsyncMessage message)
    {
        this.logger.LogInformation($"Relaying outbox messages...");

        await this.outboxRelayer.RelayMessageBatchAsync();
    }
}
