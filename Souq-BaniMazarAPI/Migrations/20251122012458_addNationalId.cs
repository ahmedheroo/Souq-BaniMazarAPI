using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Souq_BaniMazarAPI.Migrations
{
    /// <inheritdoc />
    public partial class addNationalId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NationalIdUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NationalIdUrl",
                table: "AspNetUsers");
        }
    }
}
