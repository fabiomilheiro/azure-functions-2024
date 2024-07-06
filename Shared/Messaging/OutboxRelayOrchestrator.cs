using Azf.Shared.Json;
using Azf.Shared.Sql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Azf.Shared.Messaging;

public interface IOutboxRelayer
{
    Task RelayMessagesAsync();
}

public class OutboxRelayer : IOutboxRelayer
{
    public const int MaxNumberOfAttempts = 10;
    public const int PageSize = 10;

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

    public async Task RelayMessagesAsync()
    {
        var outboxMessages = await this.GetNextBatchAsync();

        foreach (var outboxMessage in outboxMessages)
        {
            outboxMessage.State = OutboxMessageState.Processing;
        }

        await this.db.SaveChangesAsync();
        
        var succeeded = false;

        try
        {
            var asyncMessages =
                outboxMessages
                    .Select(om => (AsyncMessage)this.jsonService.Deserialize(om.Request,
                        GetMessageType(om)))
                    .ToArray();
            await this.queueClient.SendAsync(asyncMessages);
            succeeded = true;
        }
        catch (Exception exception)
        {
            this.logger.LogError(
                exception,
                "Could not relay {numberOfMessages} messages to the queue.",
                outboxMessages.Length);
            // TODO: set number of attempts and state.
        }

        if (succeeded)
        {
            await this.RemoveRelayedMessages(outboxMessages);
        }
        else
        {
            await this.ProcessFailedAttemptAsync(outboxMessages);
        }
    }

    private static Type GetMessageType(OutboxMessageBase message)
    {
        if (!AsyncMessageMappings.ByMessageTypeName.TryGetValue(message.RequestTypeName, out var mapping))
        {
            throw new BackendException(
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