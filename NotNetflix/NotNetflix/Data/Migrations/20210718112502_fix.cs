using Microsoft.EntityFrameworkCore.Migrations;

namespace NotNetflix.Data.Migrations
{
    public partial class fix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Utilizador_UtilizadorFK",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UtilizadorFK",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UtilizadorFK",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "g",
                column: "ConcurrencyStamp",
                value: "b862c0be-a124-4154-9b03-21917fa7cbd5");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "u",
                column: "ConcurrencyStamp",
                value: "49d1f28a-1485-4751-81a8-01a487257f21");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UtilizadorFK",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "g",
                column: "ConcurrencyStamp",
                value: "1fb4625a-87a5-428b-b47a-52570e2ae4ac");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "u",
                column: "ConcurrencyStamp",
                value: "67c0b27b-5076-496d-9da3-e7ec0422ebb1");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UtilizadorFK",
                table: "AspNetUsers",
                column: "UtilizadorFK");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Utilizador_UtilizadorFK",
                table: "AspNetUsers",
                column: "UtilizadorFK",
                principalTable: "Utilizador",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
