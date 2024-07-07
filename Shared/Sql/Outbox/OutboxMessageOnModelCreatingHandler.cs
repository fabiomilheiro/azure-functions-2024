using Azf.Shared.Sql.OnModelCreating;
using Microsoft.EntityFrameworkCore;

namespace Azf.Shared.Sql.Outbox;

public class OutboxMessageOnModelCreatingHandler : IOnModelCreatingHandler
{
    public void OnModelCreating(ModelBuilder modelBuilder)
    {
        // TODO: Remove.
    }
}
