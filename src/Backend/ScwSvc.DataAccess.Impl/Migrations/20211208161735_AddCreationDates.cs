using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScwSvc.DataAccess.Impl.Migrations
{
    public partial class AddCreationDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                schema: "scw1_sys",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                schema: "scw1_sys",
                table: "TableRefs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                schema: "scw1_sys",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                schema: "scw1_sys",
                table: "TableRefs");
        }
    }
}
