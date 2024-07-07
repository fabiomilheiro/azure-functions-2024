using Azf.Shared.Json;
using Azf.Shared.Messaging;
using Azf.Shared.Sql.ChangeHandling;
using Azf.Shared.Sql.OnModelCreating;
using Azf.Shared.Sql.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Azf.Shared.Sql
{
    public class SqlDbContextDependencies(
        DbContextOptions<SqlDbContext> options,
        IEntityChangeHandlingOrchestrator entityChangeHandlingOrchestrator,
        IOnModelCreatingOrchestrator onModelCreatingOrchestrator,
        IQueueClient queueClient,
        IJsonService jsonService,
        ILogger<SqlDbContext> logger)
    {
        public DbContextOptions<SqlDbContext> Options { get; private set; } = options;

        public IEntityChangeHandlingOrchestrator EntityChangeHandlingOrchestrator { get; private set; } = entityChangeHandlingOrchestrator;

        public IOnModelCreatingOrchestrator OnModelCreatingOrchestrator { get; private set; } = onModelCreatingOrchestrator;

        public IQueueClient QueueClient { get; private set; } = queueClient;

        public IJsonService JsonService { get; private set; } = jsonService;

        public ILogger<SqlDbContext> Logger { get; private set; } = logger;
    }

    public abstract class SqlDbContext : DbContext
    {
        public SqlDbContextDependencies Deps { get; private set; }

        public SqlDbContext(SqlDbContextDependencies deps)
        {
            this.Deps = deps;
        }

        public DbSet<OutboxMessageBase> OutboxMessages { get; set; }

        public DbSet<QueueMessage> QueueMessages { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            this.Deps.EntityChangeHandlingOrchestrator.Handle(this);

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
