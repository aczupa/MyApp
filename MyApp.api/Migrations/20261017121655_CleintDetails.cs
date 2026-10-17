using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.api.Migrations
{
    /// <inheritdoc />
    public partial class CleintDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Clients",
                newName: "Ville");

            migrationBuilder.AddColumn<string>(
                name: "Adresse",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CodePostal",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DenominationSociale",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Pays",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RepresentantLegal",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Telephone",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Adresse",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "CodePostal",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "DenominationSociale",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Pays",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "RepresentantLegal",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Telephone",
                table: "Clients");

            migrationBuilder.RenameColumn(
                name: "Ville",
                table: "Clients",
                newName: "Name");
        }
    }
}
