using Azf.Shared.Sql.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Azf.Shared.Sql.Outbox;

[EntityTypeConfiguration<OutboxMessageBaseConfiguration, OutboxMessageBase>]
public abstract class OutboxMessageBase : ICreatedAt, IUpdatedAt
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid RowId { get; set; }

    [ConcurrencyCheck]
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public required string MessageId { get; set; }

    public OutboxMessageType Type { get; set; }

    // Queue or topic name.
    public required string TargetName { get; set; }

    [ConcurrencyCheck]
    public required OutboxMessageState State { get; set; }

    public int NumberOfAttempts { get; set; }

    [Required]
    [StringLength(50_000)]
    public required string Request { get; set; }

    [Required]
    public required string RequestTypeName { get; set; }
}

public enum OutboxMessageType
{
    Queue = 0,
    Topic = 1,
}

public enum OutboxMessageState
{
    Waiting = 0,
    Processing = 1,
    MaxAttemptsReached = 2,
    Relayed = 3,
}
