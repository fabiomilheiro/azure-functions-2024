using Microsoft.EntityFrameworkCore;

namespace Azf.Shared.Sql.OnModelCreating;

public interface IOnModelCreatingHandler
{
    void OnModelCreating(ModelBuilder modelBuilder);
}