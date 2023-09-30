using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TablesideOrdering.Migrations
{
    public partial class update_user : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TableId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TableId",
                table: "AspNetUsers");
        }
    }
}
