using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Services.User.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdminNotificationsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AdminId",
                table: "Notification",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notification_AdminId",
                table: "Notification",
                column: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Admins_AdminId",
                table: "Notification",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Admins_AdminId",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_AdminId",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Notification");
        }
    }
}
