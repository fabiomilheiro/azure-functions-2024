using Azf.Shared.Sql.OnModelCreating;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azf.Shared.Sql.Outbox;

public class OutboxMessageOnModelCreatingHandler : IOnModelCreatingHandler
{
    public void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboxMessageBase>()
                    .HasKey(x => x.RowId);

        modelBuilder.Entity<OutboxMessageBase>()
                    .HasIndex(m => m.Type);

        modelBuilder.Entity<OutboxMessageBase>()
                    .Property(x => x.RowId)
                    .ValueGeneratedOnAdd();

        modelBuilder.Entity<OutboxMessageBase>()
                    .Property(x => x.State)
                    .IsConcurrencyToken();

        modelBuilder.Entity<OutboxMessageBase>()
                    .Property(x => x.UpdatedAt)
                    .IsConcurrencyToken();

        modelBuilder.Entity<OutboxMessageBase>()
                    .Property(x => x.Request)
                    .IsRequired()
                    .HasMaxLength(200000);

        modelBuilder.Entity<OutboxMessageBase>()
                    .Property(x => x.RequestTypeName)
                    .IsRequired();


        modelBuilder
            .Entity<OutboxMessageBase>()
            .HasDiscriminator(m => m.Type)
            .HasValue<QueueMessage>(OutboxMessageType.Queue);


    }
}
