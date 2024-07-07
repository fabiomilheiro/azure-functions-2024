using Azf.Shared.Sql;
using Azf.UserService.Sql.Models;
using Microsoft.EntityFrameworkCore;

namespace Azf.UserService.Sql
{
    public class UserSqlDbContext : SqlDbContext
    {
        public UserSqlDbContext(SqlDbContextDependencies deps)
            : base(deps)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("usersvc");
        }

        public DbSet<User> Users { get; set; }
    }
}
