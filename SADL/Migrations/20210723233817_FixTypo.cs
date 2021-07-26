using Microsoft.EntityFrameworkCore.Migrations;

namespace SADL.Migrations
{
    public partial class FixTypo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShoppingCardId",
                table: "LineItems");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShoppingCardId",
                table: "LineItems",
                type: "int",
                nullable: true);
        }
    }
}
