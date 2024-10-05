using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace azf.UserService.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
               $@"IF NOT EXISTS (SELECT 1 FROM sys.change_tracking_databases 
               WHERE database_id = DB_ID())
               BEGIN
                       ALTER DATABASE CURRENT
                       SET CHANGE_TRACKING = ON
                       (CHANGE_RETENTION = 2 DAYS, AUTO_CLEANUP = ON);
               END
               IF NOT EXISTS (SELECT 1 FROM sys.change_tracking_tables 
                               WHERE object_id = OBJECT_ID('usersvc.QueueMessages'))
               BEGIN
                       ALTER TABLE usersvc.QueueMessages
                       ENABLE CHANGE_TRACKING
                       WITH (TRACK_COLUMNS_UPDATED = OFF)
               END
               IF NOT EXISTS (SELECT 1 FROM sys.change_tracking_tables 
                               WHERE object_id = OBJECT_ID('usersvc.TopicMessages'))
               BEGIN
                       ALTER TABLE usersvc.TopicMessages
                       ENABLE CHANGE_TRACKING
                       WITH (TRACK_COLUMNS_UPDATED = OFF)
               END",
               suppressTransaction: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
