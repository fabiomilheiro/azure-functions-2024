using Azf.Shared.Json;
using Azf.Shared.Messaging;

namespace Azf.Shared.Sql.Outbox;

public interface IOutboxMessageService
{
    void AddQueueMessage<TMessage>(TMessage messageBody)
        where TMessage : AsyncMessage;
}

public class OutboxMessageService : IOutboxMessageService
{
    private readonly SqlDbContext db;
    private readonly IJsonService jsonService;

    public OutboxMessageService(
        SqlDbContext db,
        IJsonService jsonService)
    {
        this.db = db;
        this.jsonService = jsonService;
    }

    public void AddQueueMessage<TMessage>(TMessage message) // Change to dbcontext extension method.
        where TMessage : AsyncMessage
    {

        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        if (string.IsNullOrEmpty(message.MessageId))
        {
            message.MessageId = Guid.NewGuid().ToString();
        }

        var now = DateTime.UtcNow;
        this.db.QueueMessages.Add(new QueueMessage
        {
            MessageId = message.MessageId,
            Request = this.jsonService.Serialize(message),
            RequestTypeName = message.TypeName,
            State = OutboxMessageState.Waiting,
            Type = OutboxMessageType.Queue,
            UpdatedAt = now,
            CreatedAt = now,
        });
    }
}