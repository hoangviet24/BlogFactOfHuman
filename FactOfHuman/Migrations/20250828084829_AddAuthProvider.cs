using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FactOfHuman.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthProvider : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthProvider",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthProvider",
                table: "Users");
        }
    }
}
