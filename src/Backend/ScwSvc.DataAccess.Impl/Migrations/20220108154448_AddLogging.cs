using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScwSvc.DataAccess.Impl.Migrations
{
    public partial class AddLogging : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Log",
                schema: "scw1_sys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    TableId = table.Column<Guid>(type: "uuid", nullable: true),
                    TableLogEvent_TableId = table.Column<Guid>(type: "uuid", nullable: true),
                    TableCollaboratorLogEvent_UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CollaboratorAction = table.Column<int>(type: "integer", nullable: true),
                    TableType = table.Column<int>(type: "integer", nullable: true),
                    TableAction = table.Column<int>(type: "integer", nullable: true),
                    UserLogEvent_UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserAction = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Log",
                schema: "scw1_sys");
        }
    }
}
