using Microsoft.EntityFrameworkCore.Migrations;

namespace SADL.Migrations
{
    public partial class ShoppingCart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StorefrontId",
                table: "LineItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LineItems_StorefrontId",
                table: "LineItems",
                column: "StorefrontId");

            migrationBuilder.AddForeignKey(
                name: "FK_LineItems_Storefronts_StorefrontId",
                table: "LineItems",
                column: "StorefrontId",
                principalTable: "Storefronts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LineItems_Storefronts_StorefrontId",
                table: "LineItems");

            migrationBuilder.DropIndex(
                name: "IX_LineItems_StorefrontId",
                table: "LineItems");

            migrationBuilder.DropColumn(
                name: "StorefrontId",
                table: "LineItems");
        }
    }
}
