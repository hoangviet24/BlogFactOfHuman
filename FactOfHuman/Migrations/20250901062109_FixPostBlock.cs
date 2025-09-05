using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FactOfHuman.Migrations
{
    /// <inheritdoc />
    public partial class FixPostBlock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostBlock_Posts_PostId1",
                table: "PostBlock");

            migrationBuilder.DropIndex(
                name: "IX_PostBlock_PostId1",
                table: "PostBlock");

            migrationBuilder.DropColumn(
                name: "PostId1",
                table: "PostBlock");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PostId1",
                table: "PostBlock",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostBlock_PostId1",
                table: "PostBlock",
                column: "PostId1");

            migrationBuilder.AddForeignKey(
                name: "FK_PostBlock_Posts_PostId1",
                table: "PostBlock",
                column: "PostId1",
                principalTable: "Posts",
                principalColumn: "Id");
        }
    }
}
