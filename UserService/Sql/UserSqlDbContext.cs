using Azf.Shared.Sql;
using Azf.UserService.Sql.Models;
using Microsoft.EntityFrameworkCore;

namespace Azf.UserService.Sql
{
    public class UserSqlDbContext : SqlDbContext
    {
        public const string Schema = "usersvc";

        public UserSqlDbContext(SqlDbContextDependencies deps)
            : base(deps)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);
        }

        public DbSet<User> Users { get; set; }
    }
}
