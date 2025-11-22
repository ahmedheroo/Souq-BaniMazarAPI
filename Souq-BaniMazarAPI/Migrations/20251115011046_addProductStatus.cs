using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Souq_BaniMazarAPI.Migrations
{
    /// <inheritdoc />
    public partial class addProductStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Products",
                newName: "StatusId");

            migrationBuilder.CreateTable(
                name: "ProductStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductStatus", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_StatusId",
                table: "Products",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductStatus_StatusId",
                table: "Products",
                column: "StatusId",
                principalTable: "ProductStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductStatus_StatusId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "ProductStatus");

            migrationBuilder.DropIndex(
                name: "IX_Products_StatusId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "Products",
                newName: "CategoryId");
        }
    }
}
