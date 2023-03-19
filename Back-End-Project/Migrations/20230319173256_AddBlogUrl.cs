using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Back_End_Project.Migrations
{
    public partial class AddBlogUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Blogs",
                type: "nvarchar(220)",
                maxLength: 220,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                table: "Blogs");
        }
    }
}
