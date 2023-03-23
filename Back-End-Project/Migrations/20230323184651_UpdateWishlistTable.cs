using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Back_End_Project.Migrations
{
    public partial class UpdateWishlistTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "Wishlist",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "Wishlist");
        }
    }
}
