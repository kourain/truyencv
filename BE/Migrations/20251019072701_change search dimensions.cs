using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TruyenCV.Migrations
{
    /// <inheritdoc />
    public partial class changesearchdimensions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "banner_url",
                table: "comics",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cover_url",
                table: "comics",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "comic_recommends",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    comic_id = table.Column<long>(type: "bigint", nullable: false),
                    rcm_count = table.Column<long>(type: "bigint", nullable: false),
                    month = table.Column<int>(type: "integer", nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comic_recommends", x => x.id);
                    table.ForeignKey(
                        name: "FK_comic_recommends_comics_comic_id",
                        column: x => x.comic_id,
                        principalTable: "comics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comic_reports",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    comic_id = table.Column<long>(type: "bigint", nullable: false),
                    chapter_id = table.Column<long>(type: "bigint", nullable: true),
                    comment_id = table.Column<long>(type: "bigint", nullable: true),
                    reporter_id = table.Column<long>(type: "bigint", nullable: false),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comic_reports", x => x.id);
                    table.ForeignKey(
                        name: "FK_comic_reports_comic_chapters_chapter_id",
                        column: x => x.chapter_id,
                        principalTable: "comic_chapters",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_comic_reports_comic_comments_comment_id",
                        column: x => x.comment_id,
                        principalTable: "comic_comments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_comic_reports_comics_comic_id",
                        column: x => x.comic_id,
                        principalTable: "comics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_comic_reports_users_reporter_id",
                        column: x => x.reporter_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_comic_recommends_comic_id_month_year",
                table: "comic_recommends",
                columns: new[] { "comic_id", "month", "year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_comic_reports_chapter_id",
                table: "comic_reports",
                column: "chapter_id");

            migrationBuilder.CreateIndex(
                name: "IX_comic_reports_comic_id_chapter_id_comment_id",
                table: "comic_reports",
                columns: new[] { "comic_id", "chapter_id", "comment_id" });

            migrationBuilder.CreateIndex(
                name: "IX_comic_reports_comment_id",
                table: "comic_reports",
                column: "comment_id");

            migrationBuilder.CreateIndex(
                name: "IX_comic_reports_reporter_id",
                table: "comic_reports",
                column: "reporter_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comic_recommends");

            migrationBuilder.DropTable(
                name: "comic_reports");

            migrationBuilder.DropColumn(
                name: "banner_url",
                table: "comics");

            migrationBuilder.DropColumn(
                name: "cover_url",
                table: "comics");
        }
    }
}
