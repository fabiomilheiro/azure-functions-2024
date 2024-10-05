using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace azf.UserService.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "usersvc",
                table: "TopicMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "usersvc",
                table: "QueueMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                schema: "usersvc",
                table: "TopicMessages");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "usersvc",
                table: "QueueMessages");
        }
    }
}
