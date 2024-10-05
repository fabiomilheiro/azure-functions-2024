using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Azf.UserService.Migrations
{
    /// <inheritdoc />
    public partial class queue_topic_messages_separate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OutboxMessages",
                schema: "usersvc",
                table: "OutboxMessages");

            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_Type",
                schema: "usersvc",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "usersvc",
                table: "OutboxMessages");

            migrationBuilder.RenameTable(
                name: "OutboxMessages",
                schema: "usersvc",
                newName: "QueueMessages",
                newSchema: "usersvc");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QueueMessages",
                schema: "usersvc",
                table: "QueueMessages",
                column: "RowId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TopicMessages",
                schema: "usersvc");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QueueMessages",
                schema: "usersvc",
                table: "QueueMessages");

            migrationBuilder.RenameTable(
                name: "QueueMessages",
                schema: "usersvc",
                newName: "OutboxMessages",
                newSchema: "usersvc");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "usersvc",
                table: "OutboxMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutboxMessages",
                schema: "usersvc",
                table: "OutboxMessages",
                column: "RowId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_Type",
                schema: "usersvc",
                table: "OutboxMessages",
                column: "Type");
        }
    }
}
