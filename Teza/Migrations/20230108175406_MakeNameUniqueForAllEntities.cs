using Microsoft.EntityFrameworkCore.Migrations;

namespace Teza.Migrations
{
    public partial class MakeNameUniqueForAllEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Query_Name",
                table: "Query",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Folder_Name",
                table: "Folder",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Collection_Name",
                table: "Collection",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Query_Name",
                table: "Query");

            migrationBuilder.DropIndex(
                name: "IX_Folder_Name",
                table: "Folder");

            migrationBuilder.DropIndex(
                name: "IX_Collection_Name",
                table: "Collection");
        }
    }
}
