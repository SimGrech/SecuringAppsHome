using Microsoft.EntityFrameworkCore.Migrations;

namespace ShoppingCart.Data.Migrations
{
    public partial class addedtaskinfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AssignmentTasks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "AssignmentTasks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "AssignmentTasks");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "AssignmentTasks");
        }
    }
}
