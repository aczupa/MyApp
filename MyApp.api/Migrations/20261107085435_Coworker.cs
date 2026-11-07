using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.api.Migrations
{
    /// <inheritdoc />
    public partial class Coworker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CoworkerId",
                table: "Clients",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Coworkers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coworkers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coworkers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_CoworkerId",
                table: "Clients",
                column: "CoworkerId");

            migrationBuilder.CreateIndex(
                name: "IX_Coworkers_UserId",
                table: "Coworkers",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Coworkers_CoworkerId",
                table: "Clients",
                column: "CoworkerId",
                principalTable: "Coworkers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Coworkers_CoworkerId",
                table: "Clients");

            migrationBuilder.DropTable(
                name: "Coworkers");

            migrationBuilder.DropIndex(
                name: "IX_Clients_CoworkerId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "CoworkerId",
                table: "Clients");
        }
    }
}
