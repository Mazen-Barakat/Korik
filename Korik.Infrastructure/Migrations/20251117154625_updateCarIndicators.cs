using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Korik.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateCarIndicators : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarIndicators_Cars_CarId",
                table: "CarIndicators");

            migrationBuilder.DropIndex(
                name: "IX_CarIndicators_CarId_IndicatorType",
                table: "CarIndicators");

            migrationBuilder.AlterColumn<string>(
                name: "IndicatorType",
                table: "CarIndicators",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "CarStatus",
                table: "CarIndicators",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "MileageDifference",
                table: "CarIndicators",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MileageDifferenceAsPercentage",
                table: "CarIndicators",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NextMileage",
                table: "CarIndicators",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "TimeDifference",
                table: "CarIndicators",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<double>(
                name: "TimeDifferenceAsPercentage",
                table: "CarIndicators",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_CarIndicators_CarId",
                table: "CarIndicators",
                column: "CarId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarIndicators_Cars_CarId",
                table: "CarIndicators",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarIndicators_Cars_CarId",
                table: "CarIndicators");

            migrationBuilder.DropIndex(
                name: "IX_CarIndicators_CarId",
                table: "CarIndicators");

            migrationBuilder.DropColumn(
                name: "MileageDifference",
                table: "CarIndicators");

            migrationBuilder.DropColumn(
                name: "MileageDifferenceAsPercentage",
                table: "CarIndicators");

            migrationBuilder.DropColumn(
                name: "NextMileage",
                table: "CarIndicators");

            migrationBuilder.DropColumn(
                name: "TimeDifference",
                table: "CarIndicators");

            migrationBuilder.DropColumn(
                name: "TimeDifferenceAsPercentage",
                table: "CarIndicators");

            migrationBuilder.AlterColumn<string>(
                name: "IndicatorType",
                table: "CarIndicators",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CarStatus",
                table: "CarIndicators",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_CarIndicators_CarId_IndicatorType",
                table: "CarIndicators",
                columns: new[] { "CarId", "IndicatorType" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CarIndicators_Cars_CarId",
                table: "CarIndicators",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id");
        }
    }
}
