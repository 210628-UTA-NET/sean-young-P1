using Microsoft.EntityFrameworkCore.Migrations;

namespace SADL.Migrations
{
    public partial class RemoveAddressCountry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Countries_CountryAlpha2",
                table: "Addresses");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_CountryAlpha2",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "CountryAlpha2",
                table: "Addresses");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CountryAlpha2",
                table: "Addresses",
                type: "nvarchar(2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Alpha2 = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Alpha2);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CountryAlpha2",
                table: "Addresses",
                column: "CountryAlpha2");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Countries_CountryAlpha2",
                table: "Addresses",
                column: "CountryAlpha2",
                principalTable: "Countries",
                principalColumn: "Alpha2",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
