using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TruyenCV.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRecommendConstraintToPerComic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_user_comic_recommends_user_id_month_year",
                table: "user_comic_recommends");

            migrationBuilder.CreateIndex(
                name: "IX_user_comic_recommends_user_id_comic_id_month_year",
                table: "user_comic_recommends",
                columns: new[] { "user_id", "comic_id", "month", "year" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_user_comic_recommends_user_id_comic_id_month_year",
                table: "user_comic_recommends");

            migrationBuilder.CreateIndex(
                name: "IX_user_comic_recommends_user_id_month_year",
                table: "user_comic_recommends",
                columns: new[] { "user_id", "month", "year" },
                unique: true);
        }
    }
}
