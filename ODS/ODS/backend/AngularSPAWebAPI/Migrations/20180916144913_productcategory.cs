using Microsoft.EntityFrameworkCore.Migrations;

namespace AngularSPAWebAPI.Migrations
{
    public partial class productcategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductCategoryID",
                table: "SubProducts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubProducts_ProductCategoryID",
                table: "SubProducts",
                column: "ProductCategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_SubProducts_productCategories_ProductCategoryID",
                table: "SubProducts",
                column: "ProductCategoryID",
                principalTable: "productCategories",
                principalColumn: "ProductCategoryID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubProducts_productCategories_ProductCategoryID",
                table: "SubProducts");

            migrationBuilder.DropIndex(
                name: "IX_SubProducts_ProductCategoryID",
                table: "SubProducts");

            migrationBuilder.DropColumn(
                name: "ProductCategoryID",
                table: "SubProducts");
        }
    }
}
