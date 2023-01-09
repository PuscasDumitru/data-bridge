using Microsoft.EntityFrameworkCore.Migrations;

namespace Teza.Migrations
{
    public partial class MakeWksNameUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Workspace_Name",
                table: "Workspace",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Workspace_Name",
                table: "Workspace");
        }
    }
}
