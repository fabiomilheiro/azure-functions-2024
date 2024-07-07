﻿using Azf.Shared.Sql.Models;
using Microsoft.EntityFrameworkCore;

namespace Azf.Shared.Sql.Outbox;

[EntityTypeConfiguration<OutboxMessageBaseConfiguration, OutboxMessageBase>]
public abstract class OutboxMessageBase : ICreatedAt, IUpdatedAt
{
    public Guid RowId { get; set; }

    public required string MessageId { get; set; }

    public required OutboxMessageType Type { get; set; }

    // Queue or topic name.
    public required string TargetName { get; set; }

    public required OutboxMessageState State { get; set; }

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