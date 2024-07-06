using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Azf.Shared.Sql.ChangeHandling
{
    public interface IEntityChangeHandler
    {
        Type EntityType { get; }

        void Handle(EntityEntry entry);
    }
}