using Microsoft.EntityFrameworkCore.Migrations;

namespace Teza.Migrations
{
    public partial class AddDocumentationToQueryAndRemoveFromWorkspace : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Documentation",
                table: "Workspace");

            migrationBuilder.AddColumn<string>(
                name: "Documentation",
                table: "Query",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Documentation",
                table: "Query");

            migrationBuilder.AddColumn<string>(
                name: "Documentation",
                table: "Workspace",
                type: "text",
                nullable: true);
        }
    }
}
