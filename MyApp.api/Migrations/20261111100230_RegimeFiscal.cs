using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.api.Migrations
{
    /// <inheritdoc />
    public partial class RegimeFiscal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModeImposition",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "NonRenseigne");

            migrationBuilder.AddColumn<string>(
                name: "RegimeFiscal",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "NonRenseigne");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModeImposition",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "RegimeFiscal",
                table: "Clients");
        }
    }
}
