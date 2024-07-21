using Azf.Shared.Sql.Outbox;
using Microsoft.Extensions.Logging;

namespace Azf.Shared.Messaging.CommonHandlers;

public class RelayOutboxMessagesAsyncMessage
    : AsyncMessage
{
    public bool RelayQueueOutboxMessags { get; set; }

    public bool RelayTopicOutboxMessags { get; set; }
}

// TODO: Do I still need this if we use the change tracker?
public class RelayOutboxMessagesAsyncMessageHandler : AsyncMessageHandlerBase<RelayOutboxMessagesAsyncMessage>
{
    private readonly IOutboxRelayer<QueueMessage> queueOutboxRelayer;
    private readonly IOutboxRelayer<TopicMessage> topicOutboxRelayer;
    private readonly ILogger<RelayOutboxMessagesAsyncMessageHandler> logger;

    public RelayOutboxMessagesAsyncMessageHandler(
        IOutboxRelayer<QueueMessage> queueOutboxRelayer,
        IOutboxRelayer<TopicMessage> topicOutboxRelayer,
        ILogger<RelayOutboxMessagesAsyncMessageHandler> logger)
    {
        this.queueOutboxRelayer = queueOutboxRelayer;
        this.topicOutboxRelayer = topicOutboxRelayer;
        this.logger = logger;
    }

    protected override async Task HandleAsync(RelayOutboxMessagesAsyncMessage message)
    {
        if (message.RelayQueueOutboxMessags)
        {
        this.logger.LogInformation($"Relaying queue outbox messages...");
            await this.queueOutboxRelayer.RelayMessageBatchAsync();
        }

        if (message.RelayTopicOutboxMessags)
        {
        this.logger.LogInformation($"Relaying topic outbox messages...");
            await this.topicOutboxRelayer.RelayMessageBatchAsync();
        }
    }
}
