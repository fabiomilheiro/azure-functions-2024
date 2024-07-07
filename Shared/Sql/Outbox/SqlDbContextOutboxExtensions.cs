using Azf.Shared.Messaging;

namespace Azf.Shared.Sql.Outbox;

public static class SqlDbContextOutboxExtensions
{
    public static void AddQueueMessage<TMessage>(
        this SqlDbContext db,
        QueueName queueName,
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
            TargetName = queueName.ToString(),
            UpdatedAt = now,
            CreatedAt = now,
        });
    }
}