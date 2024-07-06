using Azf.Shared.Sql.ChangeHandling;
using Azf.Shared.Sql.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Azf.Shared.Sql
{
    public class SqlDbContext : DbContext
    {
        private readonly DbContextOptions<SqlDbContext> options;
        private readonly IEntityChangeHandlingOrchestrator entityChangeHandlingOrchestrator;
        private readonly ILogger<SqlDbContext> logger;

        public SqlDbContext(
            DbContextOptions<SqlDbContext> options,
        IEntityChangeHandlingOrchestrator entityChangeHandlingOrchestrator,
        //IOnModelCreatingOrchestrator onModelCreatingOrchestrator,
        //IQueueClient queueClient,
        ILogger<SqlDbContext> logger)
        : base(options)
        {
            this.options = options;
            this.entityChangeHandlingOrchestrator = entityChangeHandlingOrchestrator;
            this.logger = logger;
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
            //this.entityChangeHandlingOrchestrator.Handle(this);

            return new OnSaveChangesResult
            {
                //HasOutboxMessages = this.ChangeTracker.Entries<OutboxMessageBase>().Any(x => x.State == EntityState.Added),
            };
        }

        private async Task OnChangesSaved(OnSaveChangesResult onSaveChangesResult)
        {
            if (onSaveChangesResult.HasOutboxMessages)
            {
                try
                {
                    await Task.Delay(0);
                    //await this.queueClient.SendAsync(new RelayOutboxMessagesAsyncMessage());
                }
                catch (Exception e)
                {
                    logger.LogWarning(e, "Could not send relay outbox message to queue.");
                }
            }
        }

        private class OnSaveChangesResult
        {
            public bool HasOutboxMessages { get; init; }
        }
    }
}
