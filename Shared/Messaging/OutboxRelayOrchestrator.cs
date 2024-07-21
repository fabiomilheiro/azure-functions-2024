using Azf.Shared.Json;
using Azf.Shared.Sql;
using Azf.Shared.Sql.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Azf.Shared.Messaging;

public interface IOutboxRelayer<TOutboxMessage> where TOutboxMessage : OutboxMessageBase
{
    Task RelayMessageBatchAsync();

    Task RelayMessageBatchAsync(TOutboxMessage[] outboxMessages);
}

public class OutboxRelayerBase<TOutboxMessage>: IOutboxRelayer<TOutboxMessage>
    where TOutboxMessage : OutboxMessageBase
{
    public const int MaxNumberOfAttempts = 10;
    public const int PageSize = 100;

    private readonly SqlDbContext db;
    private readonly IJsonService jsonService;
    private readonly ILogger<OutboxRelayerBase<TOutboxMessage>> logger;
    private readonly IQueueClient queueClient;

    public OutboxRelayerBase(
        SqlDbContext db,
        IQueueClient queueClient,
        IJsonService jsonService,
        ILogger<OutboxRelayerBase<TOutboxMessage>> logger)
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

    public async Task RelayMessageBatchAsync(TOutboxMessage[] outboxMessages)
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

                        await this.queueClient.SendAsync(group.Key, asyncMessages);

                await this.RemoveRelayedMessages(inGroup);
            }
            catch (Exception exception)
            {
                this.logger.LogError(
                    exception,
                    "Could not relay {numberOfMessages} messages to the target {target} (type={type}).",
                    inGroup.Length,
                    group.Key,
                    typeof(TOutboxMessage).Name);

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

    private Task<TOutboxMessage[]> GetNextBatchAsync()
    {
        return this
               .db
               .Set<TOutboxMessage>()
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