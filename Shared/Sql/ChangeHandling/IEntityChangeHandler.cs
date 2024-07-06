using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Backend.App.Data.Sql.ChangeHandling
{
    public interface IEntityChangeHandler
    {
        Type EntityType { get; }

        void Handle(EntityEntry entry);
    }
}