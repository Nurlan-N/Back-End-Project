using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Back_End_Project.Migrations
{
    public partial class BlogImageNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "Blogs",
                type: "nvarchar(220)",
                maxLength: 220,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(220)",
                oldMaxLength: 220);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "Blogs",
                type: "nvarchar(220)",
                maxLength: 220,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(220)",
                oldMaxLength: 220,
                oldNullable: true);
        }
    }
}
