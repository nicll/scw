using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ScwSvc.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "scw1_sys");

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "scw1_sys",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "bytea", fixedLength: true, maxLength: 32, nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.UniqueConstraint("AK_Users_Name", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "TableRefs",
                schema: "scw1_sys",
                columns: table => new
                {
                    TableRefId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    LookupName = table.Column<Guid>(type: "uuid", fixedLength: true, maxLength: 24, nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableRefs", x => x.TableRefId);
                    table.ForeignKey(
                        name: "FK_TableRefs_Users_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalSchema: "scw1_sys",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataSetColumn",
                schema: "scw1_sys",
                columns: table => new
                {
                    TableRefId = table.Column<Guid>(type: "uuid", nullable: false),
                    Position = table.Column<byte>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Nullable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSetColumn", x => new { x.TableRefId, x.Position });
                    table.UniqueConstraint("AK_DataSetColumn_TableRefId_Name", x => new { x.TableRefId, x.Name });
                    table.ForeignKey(
                        name: "FK_DataSetColumn_TableRefs_TableRefId",
                        column: x => x.TableRefId,
                        principalSchema: "scw1_sys",
                        principalTable: "TableRefs",
                        principalColumn: "TableRefId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TableRefUser",
                schema: "scw1_sys",
                columns: table => new
                {
                    CollaborationsTableRefId = table.Column<Guid>(type: "uuid", nullable: false),
                    CollaboratorsUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableRefUser", x => new { x.CollaborationsTableRefId, x.CollaboratorsUserId });
                    table.ForeignKey(
                        name: "FK_TableRefUser_TableRefs_CollaborationsTableRefId",
                        column: x => x.CollaborationsTableRefId,
                        principalSchema: "scw1_sys",
                        principalTable: "TableRefs",
                        principalColumn: "TableRefId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TableRefUser_Users_CollaboratorsUserId",
                        column: x => x.CollaboratorsUserId,
                        principalSchema: "scw1_sys",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TableRefs_OwnerUserId",
                schema: "scw1_sys",
                table: "TableRefs",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TableRefUser_CollaboratorsUserId",
                schema: "scw1_sys",
                table: "TableRefUser",
                column: "CollaboratorsUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataSetColumn",
                schema: "scw1_sys");

            migrationBuilder.DropTable(
                name: "TableRefUser",
                schema: "scw1_sys");

            migrationBuilder.DropTable(
                name: "TableRefs",
                schema: "scw1_sys");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "scw1_sys");
        }
    }
}
