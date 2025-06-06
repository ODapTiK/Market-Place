using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.User.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class manufacturerNotificationsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ManufacturerId",
                table: "Notification",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notification_ManufacturerId",
                table: "Notification",
                column: "ManufacturerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Manufacturers_ManufacturerId",
                table: "Notification",
                column: "ManufacturerId",
                principalTable: "Manufacturers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Manufacturers_ManufacturerId",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_ManufacturerId",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "ManufacturerId",
                table: "Notification");
        }
    }
}
