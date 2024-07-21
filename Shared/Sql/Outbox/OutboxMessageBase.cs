using Azf.Shared.Sql.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Azf.Shared.Sql.Outbox;

[EntityTypeConfiguration<OutboxMessageBaseConfiguration, OutboxMessageBase>]
public abstract class OutboxMessageBase : ICreatedAt, IUpdatedAt
{
    [Key]
    public Guid RowId { get; set; }

    [ConcurrencyCheck]
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public required string MessageId { get; set; }

    // Queue or topic name.
    public required string TargetName { get; set; }

    [ConcurrencyCheck]
    public required OutboxMessageState State { get; set; }

    public int NumberOfAttempts { get; set; }

    [Required]
    [StringLength(100000)]
    public required string Request { get; set; }

    [Required]
    public required string RequestTypeName { get; set; }
}

public enum OutboxMessageState
{
    Waiting = 0,
    Processing = 1,
    MaxAttemptsReached = 2,
    Relayed = 3,
}
