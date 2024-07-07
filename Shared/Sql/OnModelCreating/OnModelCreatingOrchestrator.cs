using Microsoft.EntityFrameworkCore;

namespace Azf.Shared.Sql.OnModelCreating;

public interface IOnModelCreatingOrchestrator
{
    void Execute(ModelBuilder modelBuilder);
}

public class OnModelCreatingOrchestrator : IOnModelCreatingOrchestrator
{
    private readonly IEnumerable<IOnModelCreatingHandler> onModelCreatingHandlers;

    public OnModelCreatingOrchestrator(IEnumerable<IOnModelCreatingHandler> onModelCreatingHandlers)
    {
        this.onModelCreatingHandlers = onModelCreatingHandlers;
    }

    public void Execute(ModelBuilder modelBuilder)
    {
        foreach (var handler in onModelCreatingHandlers)
        {
            handler.OnModelCreating(modelBuilder);
        }
    }
}