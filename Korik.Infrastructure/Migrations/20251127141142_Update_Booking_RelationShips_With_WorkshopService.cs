using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Korik.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update_Booking_RelationShips_With_WorkshopService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingPhotos_Bookings_BookingId",
                table: "BookingPhotos");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Services_ServiceId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_CarExpenses_Cars_CarId",
                table: "CarExpenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Cars_CarOwnerProfiles_CarOwnerProfileId",
                table: "Cars");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkingHours_WorkShopProfiles_WorkShopProfileId",
                table: "WorkingHours");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkShopPhotos_WorkShopProfiles_WorkShopProfileId",
                table: "WorkShopPhotos");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "Bookings",
                newName: "WorkshopServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_ServiceId",
                table: "Bookings",
                newName: "IX_Bookings_WorkshopServiceId");

            migrationBuilder.AddColumn<int>(
                name: "CarOwnerProfileId",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WorkShopProfileId",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CarOwnerProfileId",
                table: "Reviews",
                column: "CarOwnerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_WorkShopProfileId",
                table: "Reviews",
                column: "WorkShopProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingPhotos_Bookings_BookingId",
                table: "BookingPhotos",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_WorkshopServices_WorkshopServiceId",
                table: "Bookings",
                column: "WorkshopServiceId",
                principalTable: "WorkshopServices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CarExpenses_Cars_CarId",
                table: "CarExpenses",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_CarOwnerProfiles_CarOwnerProfileId",
                table: "Cars",
                column: "CarOwnerProfileId",
                principalTable: "CarOwnerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_CarOwnerProfiles_CarOwnerProfileId",
                table: "Reviews",
                column: "CarOwnerProfileId",
                principalTable: "CarOwnerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_WorkShopProfiles_WorkShopProfileId",
                table: "Reviews",
                column: "WorkShopProfileId",
                principalTable: "WorkShopProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkingHours_WorkShopProfiles_WorkShopProfileId",
                table: "WorkingHours",
                column: "WorkShopProfileId",
                principalTable: "WorkShopProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkShopPhotos_WorkShopProfiles_WorkShopProfileId",
                table: "WorkShopPhotos",
                column: "WorkShopProfileId",
                principalTable: "WorkShopProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingPhotos_Bookings_BookingId",
                table: "BookingPhotos");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_WorkshopServices_WorkshopServiceId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_CarExpenses_Cars_CarId",
                table: "CarExpenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Cars_CarOwnerProfiles_CarOwnerProfileId",
                table: "Cars");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_CarOwnerProfiles_CarOwnerProfileId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_WorkShopProfiles_WorkShopProfileId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkingHours_WorkShopProfiles_WorkShopProfileId",
                table: "WorkingHours");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkShopPhotos_WorkShopProfiles_WorkShopProfileId",
                table: "WorkShopPhotos");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_CarOwnerProfileId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_WorkShopProfileId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "CarOwnerProfileId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "WorkShopProfileId",
                table: "Reviews");

            migrationBuilder.RenameColumn(
                name: "WorkshopServiceId",
                table: "Bookings",
                newName: "ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_WorkshopServiceId",
                table: "Bookings",
                newName: "IX_Bookings_ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingPhotos_Bookings_BookingId",
                table: "BookingPhotos",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Services_ServiceId",
                table: "Bookings",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CarExpenses_Cars_CarId",
                table: "CarExpenses",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_CarOwnerProfiles_CarOwnerProfileId",
                table: "Cars",
                column: "CarOwnerProfileId",
                principalTable: "CarOwnerProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkingHours_WorkShopProfiles_WorkShopProfileId",
                table: "WorkingHours",
                column: "WorkShopProfileId",
                principalTable: "WorkShopProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkShopPhotos_WorkShopProfiles_WorkShopProfileId",
                table: "WorkShopPhotos",
                column: "WorkShopProfileId",
                principalTable: "WorkShopProfiles",
                principalColumn: "Id");
        }
    }
}
