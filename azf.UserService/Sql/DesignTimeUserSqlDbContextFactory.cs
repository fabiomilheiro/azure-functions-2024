using Azf.Shared.Sql;
using Microsoft.EntityFrameworkCore.Design;

namespace Azf.UserService.Sql
{
    public class DesignTimeUserSqlDbContextFactory : DesignTimeSqlDbContextFactory<UserSqlDbContext>, IDesignTimeDbContextFactory<UserSqlDbContext>
    {
    }
}
