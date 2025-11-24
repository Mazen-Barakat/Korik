using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Korik.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedtheWorkshopServiceandServiceentities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "MaxPrice",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "MinPrice",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "TargetOrigin",
                table: "Services");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "WorkshopServices",
                newName: "MinPrice");

            migrationBuilder.AddColumn<decimal>(
                name: "MaxPrice",
                table: "WorkshopServices",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Origin",
                table: "WorkshopServices",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxPrice",
                table: "WorkshopServices");

            migrationBuilder.DropColumn(
                name: "Origin",
                table: "WorkshopServices");

            migrationBuilder.RenameColumn(
                name: "MinPrice",
                table: "WorkshopServices",
                newName: "Price");

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Services",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxPrice",
                table: "Services",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinPrice",
                table: "Services",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TargetOrigin",
                table: "Services",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
