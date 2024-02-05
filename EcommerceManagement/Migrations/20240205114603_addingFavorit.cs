using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceManagement.Migrations
{
    /// <inheritdoc />
    public partial class addingFavorit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Favorit",
                columns: table => new
                {
                    FavoritId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductRefId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserRefId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorit", x => x.FavoritId);
                    table.ForeignKey(
                        name: "FK_Favorit_AspNetUsers_UserRefId",
                        column: x => x.UserRefId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Favorit_Product_ProductRefId",
                        column: x => x.ProductRefId,
                        principalTable: "Product",
                        principalColumn: "ProductId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Favorit_ProductRefId",
                table: "Favorit",
                column: "ProductRefId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Favorit_UserRefId",
                table: "Favorit",
                column: "UserRefId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Favorit");
        }
    }
}
