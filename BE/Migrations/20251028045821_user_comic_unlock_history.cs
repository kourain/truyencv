using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TruyenCV.Migrations
{
    /// <inheritdoc />
    public partial class user_comic_unlock_history : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_comic_unlock_history",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    comic_id = table.Column<long>(type: "bigint", nullable: false),
                    comic_chapter_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_comic_unlock_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_comic_unlock_history_comic_chapters_comic_chapter_id",
                        column: x => x.comic_chapter_id,
                        principalTable: "comic_chapters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_comic_unlock_history_comics_comic_id",
                        column: x => x.comic_id,
                        principalTable: "comics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_comic_unlock_history_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_comic_unlock_history_comic_chapter_id",
                table: "user_comic_unlock_history",
                column: "comic_chapter_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_comic_unlock_history_comic_id",
                table: "user_comic_unlock_history",
                column: "comic_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_comic_unlock_history_user_id_comic_id_comic_chapter_id",
                table: "user_comic_unlock_history",
                columns: new[] { "user_id", "comic_id", "comic_chapter_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserComicUnlockHistory",
                table: "user_comic_unlock_history",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_comic_unlock_history");
        }
    }
}
