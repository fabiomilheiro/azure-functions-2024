using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Azf.Shared.Sql.Outbox
{
    internal class OutboxMessageBaseConfiguration
        : IEntityTypeConfiguration<OutboxMessageBase>,
        IEntityTypeConfiguration<QueueMessage>,
        IEntityTypeConfiguration<TopicMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessageBase> builder)
        {
            builder
                .HasDiscriminator(m => m.Type)
                .HasValue<QueueMessage>(OutboxMessageType.Queue)
                .HasValue<QueueMessage>(OutboxMessageType.Topic);
        }

        public void Configure(EntityTypeBuilder<QueueMessage> builder)
        {
        }

        public void Configure(EntityTypeBuilder<TopicMessage> builder)
        {
        }
    }
}
