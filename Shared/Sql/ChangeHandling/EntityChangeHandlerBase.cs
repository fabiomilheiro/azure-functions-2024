using Backend.App.Data.Sql.ChangeHandling;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Azf.Shared.Sql.ChangeHandling;

public abstract class EntityChangeHandlerBase<TEntity> : IEntityChangeHandler where TEntity : class
{
    public Type EntityType { get; } = typeof(TEntity);

    public void Handle(EntityEntry entry)
    {
        if (!entry.Entity.GetType().IsAssignableTo(EntityType))
        {
            throw new InvalidOperationException("Entry must match the entity change handler.");
        }

        var entity = (TEntity)entry.Entity;

        if (ShouldHandle(entry, entity))
        {
            Handle(entry, entity);
        }
    }

    protected abstract bool ShouldHandle(EntityEntry entry, TEntity entity);

    protected abstract void Handle(EntityEntry entry, TEntity entity);
}