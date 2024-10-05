using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace azf.UserService.Migrations
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
                    Request = table.Column<string>(type: "nvarchar(max)", maxLength: 100000, nullable: false),
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
                    Request = table.Column<string>(type: "nvarchar(max)", maxLength: 100000, nullable: false),
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
