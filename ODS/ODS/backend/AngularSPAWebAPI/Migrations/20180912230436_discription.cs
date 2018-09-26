using Microsoft.EntityFrameworkCore.Migrations;

namespace AngularSPAWebAPI.Migrations
{
    public partial class discription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "SubProducts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "SubProducts");
        }
    }
}
