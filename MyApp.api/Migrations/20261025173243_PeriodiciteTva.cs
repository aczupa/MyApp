using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.api.Migrations
{
    /// <inheritdoc />
    public partial class PeriodiciteTva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JourTva",
                table: "Clients",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PeriodiciteTva",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JourTva",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "PeriodiciteTva",
                table: "Clients");
        }
    }
}
