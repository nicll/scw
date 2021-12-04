using Microsoft.EntityFrameworkCore.Migrations;

namespace ScwSvc.DataAccess.Impl.Migrations
{
    public partial class RenameTableType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                schema: "scw1_sys",
                table: "TableRefs",
                newName: "TableType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TableType",
                schema: "scw1_sys",
                table: "TableRefs",
                newName: "Type");
        }
    }
}
