using Microsoft.EntityFrameworkCore.Migrations;

namespace ScwSvc.DataAccess.Impl.Migrations;

public partial class UnkeyUsername : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropUniqueConstraint(
            name: "AK_Users_Name",
            schema: "scw1_sys",
            table: "Users");

        migrationBuilder.CreateIndex(
            name: "IX_Users_Name",
            schema: "scw1_sys",
            table: "Users",
            column: "Name",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Users_Name",
            schema: "scw1_sys",
            table: "Users");

        migrationBuilder.AddUniqueConstraint(
            name: "AK_Users_Name",
            schema: "scw1_sys",
            table: "Users",
            column: "Name");
    }
}
