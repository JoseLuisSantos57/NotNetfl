using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NotNetflix.Data.Migrations
{
    public partial class _202105261020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Filme",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duracao = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Filme", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Utilizador",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    N_telemovel = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilizador", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fotografia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilmeFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fotografia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fotografia_Filme_FilmeFK",
                        column: x => x.FilmeFK,
                        principalTable: "Filme",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Genero",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Genre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilmeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genero", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Genero_Filme_FilmeId",
                        column: x => x.FilmeId,
                        principalTable: "Filme",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FilmeUtilizador",
                columns: table => new
                {
                    ListasDeFilmesId = table.Column<int>(type: "int", nullable: false),
                    ListasDeUtilizadoresId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilmeUtilizador", x => new { x.ListasDeFilmesId, x.ListasDeUtilizadoresId });
                    table.ForeignKey(
                        name: "FK_FilmeUtilizador_Filme_ListasDeFilmesId",
                        column: x => x.ListasDeFilmesId,
                        principalTable: "Filme",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FilmeUtilizador_Utilizador_ListasDeUtilizadoresId",
                        column: x => x.ListasDeUtilizadoresId,
                        principalTable: "Utilizador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UtilizadorFilme",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilmeFK = table.Column<int>(type: "int", nullable: false),
                    UtilizadorFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtilizadorFilme", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UtilizadorFilme_Filme_FilmeFK",
                        column: x => x.FilmeFK,
                        principalTable: "Filme",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UtilizadorFilme_Utilizador_UtilizadorFK",
                        column: x => x.UtilizadorFK,
                        principalTable: "Utilizador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GeneroFilme",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilmeFK = table.Column<int>(type: "int", nullable: false),
                    GeneroFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneroFilme", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneroFilme_Filme_FilmeFK",
                        column: x => x.FilmeFK,
                        principalTable: "Filme",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GeneroFilme_Genero_GeneroFK",
                        column: x => x.GeneroFK,
                        principalTable: "Genero",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilmeUtilizador_ListasDeUtilizadoresId",
                table: "FilmeUtilizador",
                column: "ListasDeUtilizadoresId");

            migrationBuilder.CreateIndex(
                name: "IX_Fotografia_FilmeFK",
                table: "Fotografia",
                column: "FilmeFK");

            migrationBuilder.CreateIndex(
                name: "IX_Genero_FilmeId",
                table: "Genero",
                column: "FilmeId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneroFilme_FilmeFK",
                table: "GeneroFilme",
                column: "FilmeFK");

            migrationBuilder.CreateIndex(
                name: "IX_GeneroFilme_GeneroFK",
                table: "GeneroFilme",
                column: "GeneroFK");

            migrationBuilder.CreateIndex(
                name: "IX_UtilizadorFilme_FilmeFK",
                table: "UtilizadorFilme",
                column: "FilmeFK");

            migrationBuilder.CreateIndex(
                name: "IX_UtilizadorFilme_UtilizadorFK",
                table: "UtilizadorFilme",
                column: "UtilizadorFK");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilmeUtilizador");

            migrationBuilder.DropTable(
                name: "Fotografia");

            migrationBuilder.DropTable(
                name: "GeneroFilme");

            migrationBuilder.DropTable(
                name: "UtilizadorFilme");

            migrationBuilder.DropTable(
                name: "Genero");

            migrationBuilder.DropTable(
                name: "Utilizador");

            migrationBuilder.DropTable(
                name: "Filme");
        }
    }
}
