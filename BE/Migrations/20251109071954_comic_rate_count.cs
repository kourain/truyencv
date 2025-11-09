using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TruyenCV.Migrations
{
    /// <inheritdoc />
    public partial class comic_rate_count : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "rate_count",
                table: "comics",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_user_comic_read_history_chapter_id",
                table: "user_comic_read_history",
                column: "chapter_id");

            migrationBuilder.AddForeignKey(
                name: "FK_user_comic_read_history_comic_chapters_chapter_id",
                table: "user_comic_read_history",
                column: "chapter_id",
                principalTable: "comic_chapters",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_comic_read_history_comic_chapters_chapter_id",
                table: "user_comic_read_history");

            migrationBuilder.DropIndex(
                name: "IX_user_comic_read_history_chapter_id",
                table: "user_comic_read_history");

            migrationBuilder.DropColumn(
                name: "rate_count",
                table: "comics");
        }
    }
}
