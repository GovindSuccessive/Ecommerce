using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceManagement.Migrations
{
    /// <inheritdoc />
    public partial class applyIsActiveonUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActivate",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActivate",
                table: "AspNetUsers");
        }
    }
}
