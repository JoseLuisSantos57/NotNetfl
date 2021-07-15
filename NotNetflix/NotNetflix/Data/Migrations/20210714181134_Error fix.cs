using Microsoft.EntityFrameworkCore.Migrations;

namespace NotNetflix.Data.Migrations
{
    public partial class Errorfix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "u");

            migrationBuilder.AlterColumn<string>(
                name: "Duracao",
                table: "Filme",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "g",
                column: "ConcurrencyStamp",
                value: "967be857-cdbf-493a-bfde-f494e47a4b58");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Duracao",
                table: "Filme",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "g",
                column: "ConcurrencyStamp",
                value: "d4cbc318-269f-44c9-9b44-c2055a3dde1c");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "u", "2aca5b8b-7869-432c-8e5b-9e079c701fd1", "Utilizador", "UTILIZADOR" });
        }
    }
}
