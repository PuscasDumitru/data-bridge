using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Teza.Migrations
{
    public partial class RemoveWorkspaceUserManyToMany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserWorkspace");

            migrationBuilder.AddColumn<Guid>(
                name: "WorkspaceId",
                table: "User",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_WorkspaceId",
                table: "User",
                column: "WorkspaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Workspace_WorkspaceId",
                table: "User",
                column: "WorkspaceId",
                principalTable: "Workspace",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Workspace_WorkspaceId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_WorkspaceId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "WorkspaceId",
                table: "User");

            migrationBuilder.CreateTable(
                name: "UserWorkspace",
                columns: table => new
                {
                    CollaboratorsId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspacesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWorkspace", x => new { x.CollaboratorsId, x.WorkspacesId });
                    table.ForeignKey(
                        name: "FK_UserWorkspace_User_CollaboratorsId",
                        column: x => x.CollaboratorsId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserWorkspace_Workspace_WorkspacesId",
                        column: x => x.WorkspacesId,
                        principalTable: "Workspace",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkspace_WorkspacesId",
                table: "UserWorkspace",
                column: "WorkspacesId");
        }
    }
}
