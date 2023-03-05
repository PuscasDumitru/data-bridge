using Microsoft.EntityFrameworkCore.Migrations;

namespace Teza.Migrations
{
    public partial class RemoveUniqueConstraintOnNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Workspace_Name",
                table: "Workspace");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Workspace_Name",
                table: "Workspace",
                column: "Name",
                unique: true);

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
    }
}
