using Microsoft.EntityFrameworkCore.Migrations;

namespace NotNetflix.Data.Migrations
{
    public partial class _202106091201 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Genero_Filme_FilmeId",
                table: "Genero");

            migrationBuilder.DropIndex(
                name: "IX_Genero_FilmeId",
                table: "Genero");

            migrationBuilder.DropColumn(
                name: "FilmeId",
                table: "Genero");

            migrationBuilder.AddColumn<int>(
                name: "FilmeFK",
                table: "Genero",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Path",
                table: "Fotografia",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Path",
                table: "Filme",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Filme",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "FilmeId",
                table: "Genero",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Path",
                table: "Fotografia",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Path",
                table: "Filme",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Filme",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Genero_FilmeId",
                table: "Genero",
                column: "FilmeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Genero_Filme_FilmeId",
                table: "Genero",
                column: "FilmeId",
                principalTable: "Filme",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
