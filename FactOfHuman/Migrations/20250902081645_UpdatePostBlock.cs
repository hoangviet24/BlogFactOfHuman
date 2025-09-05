using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FactOfHuman.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePostBlock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostBlock_Posts_PostId",
                table: "PostBlock");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostBlock",
                table: "PostBlock");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "PostBlock");

            migrationBuilder.RenameTable(
                name: "PostBlock",
                newName: "PostBlocks");

            migrationBuilder.RenameColumn(
                name: "content",
                table: "PostBlocks",
                newName: "TopImage");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "PostBlocks",
                newName: "TopContent");

            migrationBuilder.RenameIndex(
                name: "IX_PostBlock_PostId",
                table: "PostBlocks",
                newName: "IX_PostBlocks_PostId");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Posts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "BottomContent",
                table: "PostBlocks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BottomImage",
                table: "PostBlocks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostBlocks",
                table: "PostBlocks",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Slug",
                table: "Posts",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostBlocks_Id",
                table: "PostBlocks",
                column: "Id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PostBlocks_Posts_PostId",
                table: "PostBlocks",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostBlocks_Posts_PostId",
                table: "PostBlocks");

            migrationBuilder.DropIndex(
                name: "IX_Posts_Slug",
                table: "Posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostBlocks",
                table: "PostBlocks");

            migrationBuilder.DropIndex(
                name: "IX_PostBlocks_Id",
                table: "PostBlocks");

            migrationBuilder.DropColumn(
                name: "BottomContent",
                table: "PostBlocks");

            migrationBuilder.DropColumn(
                name: "BottomImage",
                table: "PostBlocks");

            migrationBuilder.RenameTable(
                name: "PostBlocks",
                newName: "PostBlock");

            migrationBuilder.RenameColumn(
                name: "TopImage",
                table: "PostBlock",
                newName: "content");

            migrationBuilder.RenameColumn(
                name: "TopContent",
                table: "PostBlock",
                newName: "Type");

            migrationBuilder.RenameIndex(
                name: "IX_PostBlocks_PostId",
                table: "PostBlock",
                newName: "IX_PostBlock_PostId");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "PostBlock",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostBlock",
                table: "PostBlock",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PostBlock_Posts_PostId",
                table: "PostBlock",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
