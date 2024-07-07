using Azf.Shared.Json;
using Azf.Shared.Messaging;
using Azf.Shared.Messaging.CommonHandlers;
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
        
        public IJsonService JsonService { get;private set; }   = jsonService;  

        public ILogger<SqlDbContext> Logger { get; private set; } = logger;
    }

    public abstract class SqlDbContext : DbContext
    {
        public SqlDbContextDependencies Deps { get;private set; }   

        public SqlDbContext(SqlDbContextDependencies deps)
        {
            this.Deps = deps;
        }

        public DbSet<OutboxMessageBase> OutboxMessages { get; set; }

        public DbSet<QueueMessage> QueueMessages { get; set; }

        public async Task<int> SaveChangesAsync(SaveChangesRequest request)
        {
            if (request.SkipChangeHandling)
            {
                return await base.SaveChangesAsync(request.CancellationToken);
            }

            var onSaveChangesResult = OnSaveChanges();
            var result = await base.SaveChangesAsync(request.CancellationToken);
            await OnChangesSaved(onSaveChangesResult);
            return result;
        }

        public class SaveChangesRequest
        {
            public bool SkipChangeHandling { get; set; } = false;
            public CancellationToken CancellationToken { get; set; } = new();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            return SaveChangesAsync(new SaveChangesRequest
            {
                SkipChangeHandling = false,
                CancellationToken = cancellationToken,
            });
        }

        private OnSaveChangesResult OnSaveChanges()
        {

            this.Deps.EntityChangeHandlingOrchestrator.Handle(this);

            return new OnSaveChangesResult
            {
                HasOutboxMessages = this.ChangeTracker.Entries<OutboxMessageBase>().Any(x => x.State == EntityState.Added),
            };
        }

        private async Task OnChangesSaved(OnSaveChangesResult onSaveChangesResult)
        {
            if (onSaveChangesResult.HasOutboxMessages)
            {
                try
                {
                    await this.Deps.QueueClient.SendAsync(new RelayOutboxMessagesAsyncMessage());
                }
                catch (Exception e)
                {
                    this.Deps.Logger.LogWarning(e, "Could not send relay outbox message to queue.");
                }
            }
        }

        private class OnSaveChangesResult
        {
            public bool HasOutboxMessages { get; init; }
        }
    }
}
