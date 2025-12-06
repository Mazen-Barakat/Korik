using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Korik.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InProgressPhaseNotificatiosUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CarOwnerConfirmed",
                table: "Bookings",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ConfirmationDeadline",
                table: "Bookings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ConfirmationSentAt",
                table: "Bookings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "WorkshopOwnerConfirmed",
                table: "Bookings",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarOwnerConfirmed",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ConfirmationDeadline",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ConfirmationSentAt",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "WorkshopOwnerConfirmed",
                table: "Bookings");
        }
    }
}
