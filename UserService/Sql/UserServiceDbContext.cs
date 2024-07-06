using Azf.Shared.Sql;
using Azf.UserService.Sql.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Azf.UserService.Sql
{
    internal class UserServiceDbContext : SqlDbContext
    {
        public UserServiceDbContext(DbContextOptions options)
            : base()
        {
        }
        public DbSet<User> Users { get; set; }

    }
}
