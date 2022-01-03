using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScwSvc.DataAccess.Impl.Migrations
{
    public partial class RenameTableRef : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataSetColumn_TableRefs_TableRefId",
                schema: "scw1_sys",
                table: "DataSetColumn");

            migrationBuilder.DropTable(
                name: "TableRefUser",
                schema: "scw1_sys");

            migrationBuilder.DropTable(
                name: "TableRefs",
                schema: "scw1_sys");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_DataSetColumn_TableRefId_Name",
                schema: "scw1_sys",
                table: "DataSetColumn");

            migrationBuilder.RenameColumn(
                name: "TableRefId",
                schema: "scw1_sys",
                table: "DataSetColumn",
                newName: "TableId");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_DataSetColumn_TableId_Name",
                schema: "scw1_sys",
                table: "DataSetColumn",
                columns: new[] { "TableId", "Name" });

            migrationBuilder.CreateTable(
                name: "Tables",
                schema: "scw1_sys",
                columns: table => new
                {
                    TableId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TableType = table.Column<int>(type: "integer", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LookupName = table.Column<Guid>(type: "uuid", fixedLength: true, maxLength: 24, nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tables", x => x.TableId);
                    table.ForeignKey(
                        name: "FK_Tables_Users_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalSchema: "scw1_sys",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TableUser",
                schema: "scw1_sys",
                columns: table => new
                {
                    CollaborationsTableId = table.Column<Guid>(type: "uuid", nullable: false),
                    CollaboratorsUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableUser", x => new { x.CollaborationsTableId, x.CollaboratorsUserId });
                    table.ForeignKey(
                        name: "FK_TableUser_Tables_CollaborationsTableId",
                        column: x => x.CollaborationsTableId,
                        principalSchema: "scw1_sys",
                        principalTable: "Tables",
                        principalColumn: "TableId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TableUser_Users_CollaboratorsUserId",
                        column: x => x.CollaboratorsUserId,
                        principalSchema: "scw1_sys",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tables_OwnerUserId",
                schema: "scw1_sys",
                table: "Tables",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TableUser_CollaboratorsUserId",
                schema: "scw1_sys",
                table: "TableUser",
                column: "CollaboratorsUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DataSetColumn_Tables_TableId",
                schema: "scw1_sys",
                table: "DataSetColumn",
                column: "TableId",
                principalSchema: "scw1_sys",
                principalTable: "Tables",
                principalColumn: "TableId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataSetColumn_Tables_TableId",
                schema: "scw1_sys",
                table: "DataSetColumn");

            migrationBuilder.DropTable(
                name: "TableUser",
                schema: "scw1_sys");

            migrationBuilder.DropTable(
                name: "Tables",
                schema: "scw1_sys");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_DataSetColumn_TableId_Name",
                schema: "scw1_sys",
                table: "DataSetColumn");

            migrationBuilder.RenameColumn(
                name: "TableId",
                schema: "scw1_sys",
                table: "DataSetColumn",
                newName: "TableRefId");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_DataSetColumn_TableRefId_Name",
                schema: "scw1_sys",
                table: "DataSetColumn",
                columns: new[] { "TableRefId", "Name" });

            migrationBuilder.CreateTable(
                name: "TableRefs",
                schema: "scw1_sys",
                columns: table => new
                {
                    TableRefId = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    LookupName = table.Column<Guid>(type: "uuid", fixedLength: true, maxLength: 24, nullable: false),
                    TableType = table.Column<int>(type: "integer", nullable: false)
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

            migrationBuilder.AddForeignKey(
                name: "FK_DataSetColumn_TableRefs_TableRefId",
                schema: "scw1_sys",
                table: "DataSetColumn",
                column: "TableRefId",
                principalSchema: "scw1_sys",
                principalTable: "TableRefs",
                principalColumn: "TableRefId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
