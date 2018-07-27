using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AngularSPAWebAPI.Migrations
{
    public partial class views : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Views",
                table: "OogstkaartItems");

            migrationBuilder.CreateTable(
                name: "Views",
                columns: table => new
                {
                    ViewID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    OogstkaartItemID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Views", x => x.ViewID);
                    table.ForeignKey(
                        name: "FK_Views_OogstkaartItems_OogstkaartItemID",
                        column: x => x.OogstkaartItemID,
                        principalTable: "OogstkaartItems",
                        principalColumn: "OogstkaartItemID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Views_OogstkaartItemID",
                table: "Views",
                column: "OogstkaartItemID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Views");

            migrationBuilder.AddColumn<int>(
                name: "Views",
                table: "OogstkaartItems",
                nullable: false,
                defaultValue: 0);
        }
    }
}
