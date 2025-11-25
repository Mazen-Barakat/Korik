using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Korik.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addorigintothecompositeprimarykeyofworkshopservices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkshopServices_ServiceId_WorkShopProfileId",
                table: "WorkshopServices");

            migrationBuilder.CreateIndex(
                name: "IX_WorkshopServices_ServiceId_WorkShopProfileId_Origin",
                table: "WorkshopServices",
                columns: new[] { "ServiceId", "WorkShopProfileId", "Origin" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkshopServices_ServiceId_WorkShopProfileId_Origin",
                table: "WorkshopServices");

            migrationBuilder.CreateIndex(
                name: "IX_WorkshopServices_ServiceId_WorkShopProfileId",
                table: "WorkshopServices",
                columns: new[] { "ServiceId", "WorkShopProfileId" },
                unique: true);
        }
    }
}
