using Azf.Shared.Sql.Models;
using System.ComponentModel.DataAnnotations;

namespace Azf.Shared.Sql.Outbox;

public abstract class OutboxMessageBase : ICreatedAt, IUpdatedAt
{
    public Guid RowId { get; set; }

    public required string MessageId { get; set; }

    public OutboxMessageType Type { get; set; }

    public OutboxMessageState State { get; set; }

    public int NumberOfAttempts { get; set; }

    public required string Request { get; set; }

    public required string RequestTypeName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}

public enum OutboxMessageState
{
    Waiting = 0,
    Processing = 1,
    MaxAttemptsReached = 2,
}

public enum OutboxMessageType
{
    Queue = 0,
    Topic = 1,
}