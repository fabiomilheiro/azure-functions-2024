using Azf.Shared.Json;
using Azf.Shared.Sql;
using Azf.Shared.Sql.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Azf.Shared.Messaging;

public interface IOutboxRelayer
{
    Task RelayMessageBatchAsync();

    Task RelayMessageBatchAsync(OutboxMessageBase[] outboxMessages);
}

public class OutboxRelayer : IOutboxRelayer
{
    public const int MaxNumberOfAttempts = 10;
    public const int PageSize = 100;

    private readonly SqlDbContext db;
    private readonly IJsonService jsonService;
    private readonly ILogger<OutboxRelayer> logger;
    private readonly IQueueClient queueClient;

    public OutboxRelayer(
        SqlDbContext db,
        IQueueClient queueClient,
        IJsonService jsonService,
        ILogger<OutboxRelayer> logger)
    {
        this.db = db;
        this.queueClient = queueClient;
        this.jsonService = jsonService;
        this.logger = logger;
    }

    public async Task RelayMessageBatchAsync()
    {
        var outboxMessages = await this.GetNextBatchAsync();

        await this.RelayMessageBatchAsync(outboxMessages);
    }

    // TODO: Separate outbox queue/topic messages by using EF core TPC:
    // https://learn.microsoft.com/en-us/ef/core/modeling/inheritance
    public async Task RelayMessageBatchAsync(OutboxMessageBase[] outboxMessages)
    {
        foreach (var outboxMessage in outboxMessages)
        {
            outboxMessage.State = OutboxMessageState.Processing;
        }

        await this.db.SaveChangesAsync();

        var outboxMessagesGroupedByTypeAndTargetName = outboxMessages.GroupBy(m => m.TargetName).ToArray();

        foreach (var group in outboxMessagesGroupedByTypeAndTargetName)
        {
            var inGroup = group.ToArray();

            try
            {
                var asyncMessages =
                    inGroup
                        .Select(om => (AsyncMessage)this.jsonService.Deserialize(om.Request,
                            GetMessageType(om)))
                        .ToArray();

                switch (group.Key.Type)
                {
                    case OutboxMessageType.Queue:
                        await this.queueClient.SendAsync(group.Key.TargetName, asyncMessages);
                        break;

                    default:
                        throw new NotImplementedException($"Relaying of message type '{group.Key.Type}' not supported yet");
                }

                await this.RemoveRelayedMessages(inGroup);
            }
            catch (Exception exception)
            {
                this.logger.LogError(
                    exception,
                    "Could not relay {numberOfMessages} messages to the queue.",
                    inGroup.Length);

                await this.ProcessFailedAttemptAsync(inGroup);
            }
        }
    }
    

    private static Type GetMessageType(OutboxMessageBase message)
    {
        if (!AsyncMessageMappings.ByMessageTypeName.TryGetValue(message.RequestTypeName, out var mapping))
        {
            throw new Exception(
                $"The message '{message.RequestTypeName}' could not be found.");
        }

        return mapping.MessageType;
    }

    private Task<OutboxMessageBase[]> GetNextBatchAsync()
    {
        return this
               .db
               .OutboxMessages
               .Where(m => m.State == OutboxMessageState.Waiting)
               .Take(PageSize)
               .OrderBy(m => m.CreatedAt)
               .ToArrayAsync();
    }

    private async Task RemoveRelayedMessages(OutboxMessageBase[] messagesRelayed)
    {
        this.db.RemoveRange(messagesRelayed.Cast<object>());
        await this.db.SaveChangesAsync();
    }

    private async Task ProcessFailedAttemptAsync(OutboxMessageBase[] messages)
    {
        foreach (var message in messages)
        {
            if (message.NumberOfAttempts < MaxNumberOfAttempts)
            {
                message.NumberOfAttempts++;
                message.State = OutboxMessageState.Waiting;
            }
            else
            {
                message.State = OutboxMessageState.MaxAttemptsReached;
            }
        }

        await this.db.SaveChangesAsync();
    }
}