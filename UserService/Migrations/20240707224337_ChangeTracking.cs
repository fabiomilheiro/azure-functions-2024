using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Azf.UserService.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Debugger.Launch();

            //migrationBuilder.AlterDatabase()
            migrationBuilder.Sql(
                $@"ALTER DATABASE CURRENT
                SET CHANGE_TRACKING = ON  
                (CHANGE_RETENTION = 2 DAYS, AUTO_CLEANUP = ON)
                
                ALTER TABLE usersvc.OutboxMessages  
                ENABLE CHANGE_TRACKING  
                WITH (TRACK_COLUMNS_UPDATED = ON)",
                suppressTransaction: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
