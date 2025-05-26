using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Order.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class orderPointTableUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "productCategory",
                table: "OrderPoints",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "productDescription",
                table: "OrderPoints",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "productImage",
                table: "OrderPoints",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "productName",
                table: "OrderPoints",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "productType",
                table: "OrderPoints",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "productCategory",
                table: "OrderPoints");

            migrationBuilder.DropColumn(
                name: "productDescription",
                table: "OrderPoints");

            migrationBuilder.DropColumn(
                name: "productImage",
                table: "OrderPoints");

            migrationBuilder.DropColumn(
                name: "productName",
                table: "OrderPoints");

            migrationBuilder.DropColumn(
                name: "productType",
                table: "OrderPoints");
        }
    }
}
