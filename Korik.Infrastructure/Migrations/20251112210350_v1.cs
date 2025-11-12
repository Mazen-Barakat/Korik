using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Korik.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarOwnerProfiles_AspNetUsers_ApplicationUserId1",
                table: "CarOwnerProfiles");

            migrationBuilder.DropIndex(
                name: "IX_CarOwnerProfiles_ApplicationUserId1",
                table: "CarOwnerProfiles");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "CarOwnerProfiles");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId1",
                table: "WorkShopProfiles",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkShopProfiles_ApplicationUserId1",
                table: "WorkShopProfiles",
                column: "ApplicationUserId1",
                unique: true,
                filter: "[ApplicationUserId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkShopProfiles_AspNetUsers_ApplicationUserId1",
                table: "WorkShopProfiles",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkShopProfiles_AspNetUsers_ApplicationUserId1",
                table: "WorkShopProfiles");

            migrationBuilder.DropIndex(
                name: "IX_WorkShopProfiles_ApplicationUserId1",
                table: "WorkShopProfiles");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "WorkShopProfiles");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId1",
                table: "CarOwnerProfiles",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarOwnerProfiles_ApplicationUserId1",
                table: "CarOwnerProfiles",
                column: "ApplicationUserId1",
                unique: true,
                filter: "[ApplicationUserId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_CarOwnerProfiles_AspNetUsers_ApplicationUserId1",
                table: "CarOwnerProfiles",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
