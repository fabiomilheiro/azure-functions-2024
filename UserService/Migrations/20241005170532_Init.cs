using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Azf.UserService.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "usersvc");

            migrationBuilder.CreateTable(
                name: "QueueMessages",
                schema: "usersvc",
                columns: table => new
                {
                    RowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MessageId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    NumberOfAttempts = table.Column<int>(type: "int", nullable: false),
                    Request = table.Column<string>(type: "nvarchar(max)", maxLength: 50000, nullable: false),
                    RequestTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueueMessages", x => x.RowId);
                });

            migrationBuilder.CreateTable(
                name: "TopicMessages",
                schema: "usersvc",
                columns: table => new
                {
                    RowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MessageId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    NumberOfAttempts = table.Column<int>(type: "int", nullable: false),
                    Request = table.Column<string>(type: "nvarchar(max)", maxLength: 50000, nullable: false),
                    RequestTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicMessages", x => x.RowId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "usersvc",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.change_tracking_databases 
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
                END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QueueMessages",
                schema: "usersvc");

            migrationBuilder.DropTable(
                name: "TopicMessages",
                schema: "usersvc");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "usersvc");
        }
    }
}
