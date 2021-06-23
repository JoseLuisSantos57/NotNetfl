using Microsoft.EntityFrameworkCore.Migrations;

namespace NotNetflix.Data.Migrations
{
    public partial class bugresolver : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Genero_Filme_FilmeFK",
                table: "Genero");

            migrationBuilder.DropIndex(
                name: "IX_Genero_FilmeFK",
                table: "Genero");

            migrationBuilder.DropColumn(
                name: "FilmeFK",
                table: "Genero");

            migrationBuilder.CreateTable(
                name: "FilmeGenero",
                columns: table => new
                {
                    ListaDeFilmesId = table.Column<int>(type: "int", nullable: false),
                    ListasDeGenerosId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilmeGenero", x => new { x.ListaDeFilmesId, x.ListasDeGenerosId });
                    table.ForeignKey(
                        name: "FK_FilmeGenero_Filme_ListaDeFilmesId",
                        column: x => x.ListaDeFilmesId,
                        principalTable: "Filme",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FilmeGenero_Genero_ListasDeGenerosId",
                        column: x => x.ListasDeGenerosId,
                        principalTable: "Genero",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilmeGenero_ListasDeGenerosId",
                table: "FilmeGenero",
                column: "ListasDeGenerosId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilmeGenero");

            migrationBuilder.AddColumn<int>(
                name: "FilmeFK",
                table: "Genero",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Genero_FilmeFK",
                table: "Genero",
                column: "FilmeFK");

            migrationBuilder.AddForeignKey(
                name: "FK_Genero_Filme_FilmeFK",
                table: "Genero",
                column: "FilmeFK",
                principalTable: "Filme",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
