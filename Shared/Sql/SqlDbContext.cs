using Azf.Shared.Json;
using Azf.Shared.Messaging;
using Azf.Shared.Sql.ChangeHandling;
using Azf.Shared.Sql.OnModelCreating;
using Azf.Shared.Sql.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Azf.Shared.Sql
{
    public class SqlDbContextDependencies(
        IEntityChangeHandlingOrchestrator entityChangeHandlingOrchestrator,
        IOnModelCreatingOrchestrator onModelCreatingOrchestrator,
        IQueueClient queueClient,
        IJsonService jsonService)
    {
        public IEntityChangeHandlingOrchestrator EntityChangeHandlingOrchestrator { get; private set; } = entityChangeHandlingOrchestrator;

        public IOnModelCreatingOrchestrator OnModelCreatingOrchestrator { get; private set; } = onModelCreatingOrchestrator;

        public IQueueClient QueueClient { get; private set; } = queueClient;

        public IJsonService JsonService { get; private set; } = jsonService;
    }

    public abstract class SqlDbContext : DbContext
    {
        public SqlDbContextDependencies Deps { get; private set; }

        public SqlDbContext(DbContextOptions options, SqlDbContextDependencies deps)
            : base(options)
        {
            this.Deps = deps;
        }

        public DbSet<QueueMessage> QueueMessages { get; set; }

        public DbSet<TopicMessage> TopicMessages { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            this.Deps.EntityChangeHandlingOrchestrator.Handle(this);

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
