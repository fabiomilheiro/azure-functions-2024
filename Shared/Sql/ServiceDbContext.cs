using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Sql.ChangeHandling;
using Shared.Sql.Models;
using System;

namespace Shared.Sql
{
    public class ServiceDbContext : DbContext
    {
        private readonly DbContextOptions<ServiceDbContext> options;
        private readonly IEntityChangeHandlingOrchestrator entityChangeHandlingOrchestrator;
        private readonly ILogger<ServiceDbContext> logger;

        public ServiceDbContext(
            DbContextOptions<ServiceDbContext> options,
        IEntityChangeHandlingOrchestrator entityChangeHandlingOrchestrator,
        //IOnModelCreatingOrchestrator onModelCreatingOrchestrator,
        //IQueueClient queueClient,
        ILogger<ServiceDbContext> logger)
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

            var onSaveChangesResult = this.OnSaveChanges();
            var result = await base.SaveChangesAsync(request.CancellationToken);
            await this.OnChangesSaved(onSaveChangesResult);
            return result;
        }

        public class SaveChangesRequest
        {
            public bool SkipChangeHandling { get; set; } = false;
            public CancellationToken CancellationToken { get; set; } = new();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            return this.SaveChangesAsync(new SaveChangesRequest
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
                    //await this.queueClient.SendAsync(new RelayOutboxMessagesAsyncMessage());
                }
                catch (Exception e)
                {
                    this.logger.LogWarning(e, "Could not send relay outbox message to queue.");
                }
            }
        }

        private class OnSaveChangesResult
        {
            public bool HasOutboxMessages { get; init; }
        }
    }
}
