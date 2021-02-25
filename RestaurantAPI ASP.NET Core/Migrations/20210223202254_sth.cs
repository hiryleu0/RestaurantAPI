using Microsoft.EntityFrameworkCore.Migrations;

namespace RestaurantAPI_ASP.NET_Core.Migrations
{
    public partial class sth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "sth",
                table: "Restaurants",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sth",
                table: "Restaurants");
        }
    }
}
