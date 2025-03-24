using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.Order.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderPoints_Orders_Orderid",
                table: "OrderPoints");

            migrationBuilder.RenameColumn(
                name: "Orderid",
                table: "OrderPoints",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderPoints_Orderid",
                table: "OrderPoints",
                newName: "IX_OrderPoints_OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderPoints_Orders_OrderId",
                table: "OrderPoints",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderPoints_Orders_OrderId",
                table: "OrderPoints");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "OrderPoints",
                newName: "Orderid");

            migrationBuilder.RenameIndex(
                name: "IX_OrderPoints_OrderId",
                table: "OrderPoints",
                newName: "IX_OrderPoints_Orderid");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderPoints_Orders_Orderid",
                table: "OrderPoints",
                column: "Orderid",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
