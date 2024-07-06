using Azf.UserService.Sql.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Sql;

namespace Azf.UserService.Sql
{
    internal class UserServiceDbContext : ServiceDbContext
    {
        public DbSet<User> Users { get; set; }

    }
}
