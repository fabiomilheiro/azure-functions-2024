using Azf.Shared.Json;
using Azf.Shared.Messaging;

namespace Azf.Shared.Sql.Outbox;

public interface IOutboxMessageService
{
    void AddQueueMessage<TMessage>(TMessage messageBody)
        where TMessage : AsyncMessage;
}

public static class SqlDbContextOutboxExtensions
{
    public static void AddQueueMessage<TMessage>(
        this SqlDbContext db,
        TMessage message)
        where TMessage : AsyncMessage
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));

        if (string.IsNullOrEmpty(message.MessageId))
        {
            message.MessageId = Guid.NewGuid().ToString();
        }

        var now = DateTime.UtcNow;
        db.QueueMessages.Add(new QueueMessage
        {
            MessageId = message.MessageId,
            Request = db.Deps.JsonService.Serialize(message),
            RequestTypeName = message.TypeName,
            State = OutboxMessageState.Waiting,
            Type = OutboxMessageType.Queue,
            UpdatedAt = now,
            CreatedAt = now,
        });
    }
}