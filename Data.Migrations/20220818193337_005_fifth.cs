using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApp.Data.Migrations
{
    public partial class _005_fifth : Migration
    {





        protected override void Up(MigrationBuilder migrationBuilder)
        {

       

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Comments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Comments",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: true);
        }
    }
}
