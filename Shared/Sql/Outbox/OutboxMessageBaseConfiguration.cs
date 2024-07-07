using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Azf.Shared.Sql.Outbox
{
    internal class OutboxMessageBaseConfiguration
        : IEntityTypeConfiguration<OutboxMessageBase>,
        IEntityTypeConfiguration<QueueMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessageBase> builder)
        {
            builder.HasKey(x => x.RowId);

            builder.HasIndex(m => m.Type);

            builder.Property(x => x.RowId).ValueGeneratedOnAdd();

            builder.Property(x => x.State).IsConcurrencyToken();

            builder.Property(x => x.UpdatedAt).IsConcurrencyToken();

            builder.Property(x => x.Request).IsRequired().HasMaxLength(200000);

            builder.Property(x => x.RequestTypeName).IsRequired();


            builder
                .HasDiscriminator(m => m.Type)
                .HasValue<QueueMessage>(OutboxMessageType.Queue);
        }

        public void Configure(EntityTypeBuilder<QueueMessage> builder)
        {
        }
    }
}
