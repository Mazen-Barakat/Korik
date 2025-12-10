using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Korik.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PaymentTestingOnAhmadMahmoudLaptop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    WorkshopAmount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    CommissionRate = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: false),
                    StripePaymentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StripePaymentIntentId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsPaidOut = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PayoutDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PayoutMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PayoutReference = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PayoutNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BookingId",
                table: "Payments",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_IsPaidOut",
                table: "Payments",
                column: "IsPaidOut");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_StripePaymentIntentId",
                table: "Payments",
                column: "StripePaymentIntentId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");
        }
    }
}
