using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Korik.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedTheCarOriginEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TargetOrigin",
                table: "Services",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Origin",
                table: "Cars",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TargetOrigin",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Origin",
                table: "Cars");
        }
    }
}
