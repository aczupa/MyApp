using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.api.Migrations
{
    /// <inheritdoc />
    public partial class ValidationTva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ValidationsTva",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    Annee = table.Column<int>(type: "int", nullable: false),
                    Mois = table.Column<int>(type: "int", nullable: false),
                    DateValidation = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidationsTva", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ValidationsTva_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ValidationsTva_ClientId_Annee_Mois",
                table: "ValidationsTva",
                columns: new[] { "ClientId", "Annee", "Mois" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ValidationsTva");
        }
    }
}
