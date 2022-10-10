using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Teza.Migrations
{
    public partial class InitialDBCreation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Collection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsFavorite = table.Column<bool>(type: "boolean", nullable: false),
                    ShareLink = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collection", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Query",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RawSql = table.Column<string>(type: "text", nullable: true),
                    DefaultResponseWithLimit = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    LastExecuteTime = table.Column<double>(type: "double precision", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false),
                    Size = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Query", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Workspace",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DbConnectionString = table.Column<string>(type: "text", nullable: true),
                    DefaultConfigsForQueries = table.Column<string>(type: "text", nullable: true),
                    EnvVariables = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    UsersLimit = table.Column<int>(type: "integer", nullable: false),
                    InviteLink = table.Column<string>(type: "text", nullable: true),
                    CollectionId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workspace", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workspace_Collection_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Folder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    CollectionId = table.Column<int>(type: "integer", nullable: true),
                    QueryId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Folder_Collection_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Folder_Query_QueryId",
                        column: x => x.QueryId,
                        principalTable: "Query",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "History",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Action = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History", x => x.Id);
                    table.ForeignKey(
                        name: "FK_History_Query_Id",
                        column: x => x.Id,
                        principalTable: "Query",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: true),
                    HashPassword = table.Column<string>(type: "text", nullable: true),
                    HistoryId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_History_HistoryId",
                        column: x => x.HistoryId,
                        principalTable: "History",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserWorkspace",
                columns: table => new
                {
                    UsersId = table.Column<int>(type: "integer", nullable: false),
                    WorkspacesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWorkspace", x => new { x.UsersId, x.WorkspacesId });
                    table.ForeignKey(
                        name: "FK_UserWorkspace_User_UsersId",
                        column: x => x.UsersId,
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
                name: "IX_Folder_CollectionId",
                table: "Folder",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Folder_QueryId",
                table: "Folder",
                column: "QueryId");

            migrationBuilder.CreateIndex(
                name: "IX_User_HistoryId",
                table: "User",
                column: "HistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkspace_WorkspacesId",
                table: "UserWorkspace",
                column: "WorkspacesId");

            migrationBuilder.CreateIndex(
                name: "IX_Workspace_CollectionId",
                table: "Workspace",
                column: "CollectionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Folder");

            migrationBuilder.DropTable(
                name: "UserWorkspace");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Workspace");

            migrationBuilder.DropTable(
                name: "History");

            migrationBuilder.DropTable(
                name: "Collection");

            migrationBuilder.DropTable(
                name: "Query");
        }
    }
}
