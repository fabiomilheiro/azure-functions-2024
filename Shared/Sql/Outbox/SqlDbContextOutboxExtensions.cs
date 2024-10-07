using Azf.Shared.Messaging;

namespace Azf.Shared.Sql.Outbox;

public static class SqlDbContextOutboxExtensions
{
    public static void AddOutboxQueueMessage<TMessage>(
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
            TargetName = queueName.ToString().ToLowerInvariant(),
            UpdatedAt = now,
            CreatedAt = now,
        });
    }

    public static void AddOutboxTopicMessage<TMessage>(
        this SqlDbContext db,
        TopicName queueName,
        TMessage message)
        where TMessage : AsyncMessage
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));

        if (string.IsNullOrEmpty(message.MessageId))
        {
            message.MessageId = Guid.NewGuid().ToString();
        }

        var now = DateTime.UtcNow;
        db.TopicMessages.Add(new TopicMessage
        {
            MessageId = message.MessageId,
            Request = db.Deps.JsonService.Serialize(message),
            RequestTypeName = message.TypeName,
            State = OutboxMessageState.Waiting,
            TargetName = queueName.ToString().ToLowerInvariant(),
            UpdatedAt = now,
            CreatedAt = now,
        });
    }
}