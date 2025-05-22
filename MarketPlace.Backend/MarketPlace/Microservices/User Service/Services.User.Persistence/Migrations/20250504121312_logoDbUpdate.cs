using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.User.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class logoDbUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "Manufacturers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "Admins",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Manufacturers");

            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Admins");
        }
    }
}
