using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FactOfHuman.Migrations
{
    /// <inheritdoc />
    public partial class FixComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FactId",
                table: "Comments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FactId",
                table: "Comments",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
