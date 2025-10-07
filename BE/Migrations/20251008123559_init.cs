using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TruyenCV.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "comic_categories",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comic_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "comics",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    slug = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    author = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    embedded_from = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    embedded_from_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    chap_count = table.Column<int>(type: "integer", nullable: false),
                    rate = table.Column<float>(type: "real", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comics", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    password = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    phone = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    remember_token = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "comic_chapters",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    comic_id = table.Column<long>(type: "bigint", nullable: false),
                    chapter = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comic_chapters", x => x.id);
                    table.ForeignKey(
                        name: "FK_comic_chapters_comics_comic_id",
                        column: x => x.comic_id,
                        principalTable: "comics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comic_have_categories",
                columns: table => new
                {
                    comic_id = table.Column<long>(type: "bigint", nullable: false),
                    comic_category_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comic_have_categories", x => new { x.comic_id, x.comic_category_id });
                    table.ForeignKey(
                        name: "FK_comic_have_categories_comic_categories_comic_category_id",
                        column: x => x.comic_category_id,
                        principalTable: "comic_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_comic_have_categories_comics_comic_id",
                        column: x => x.comic_id,
                        principalTable: "comics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    token = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_has_roles",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_name = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    assigned_by = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_has_roles", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_has_roles_Users_assigned_by",
                        column: x => x.assigned_by,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_has_roles_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comic_comments",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    comic_id = table.Column<long>(type: "bigint", nullable: false),
                    comic_chapter_id = table.Column<long>(type: "bigint", nullable: true),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: false),
                    like = table.Column<int>(type: "integer", nullable: false),
                    reply_id = table.Column<long>(type: "bigint", nullable: true),
                    is_rate = table.Column<bool>(type: "boolean", nullable: false),
                    rate_star = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comic_comments", x => x.id);
                    table.ForeignKey(
                        name: "FK_comic_comments_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_comic_comments_comic_chapters_comic_chapter_id",
                        column: x => x.comic_chapter_id,
                        principalTable: "comic_chapters",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_comic_comments_comic_comments_reply_id",
                        column: x => x.reply_id,
                        principalTable: "comic_comments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_comic_comments_comics_comic_id",
                        column: x => x.comic_id,
                        principalTable: "comics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "id", "created_at", "deleted_at", "email", "name", "password", "phone", "remember_token", "updated_at" },
                values: new object[,]
                {
                    { -1L, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "ht.kourain@gmail.com", "System", "$2a$12$no/pEj.h1swH6xHTpg2aIetBM4aVUdRecIw28M7py2Gr6VcFeR48K", "0000000000", null, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 1L, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "maiquyen16503@gmail.com", "kourain", "$2a$12$CBUvVjDrby8apskH4IXVy.7LBAYen1q5WOUNdTtZvCu/b0OaC8S7C", "0123456789", null, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "user_has_roles",
                columns: new[] { "id", "assigned_by", "created_at", "deleted_at", "role_name", "updated_at", "user_id" },
                values: new object[,]
                {
                    { 763741477254725632L, -1L, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Admin", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), 1L },
                    { 763741477258919936L, -1L, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "System", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), -1L }
                });

            migrationBuilder.CreateIndex(
                name: "IX_comic_categories_id",
                table: "comic_categories",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_comic_categories_name",
                table: "comic_categories",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_comic_chapters_comic_id_chapter",
                table: "comic_chapters",
                columns: new[] { "comic_id", "chapter" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_comic_chapters_id",
                table: "comic_chapters",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_comic_comments_comic_chapter_id",
                table: "comic_comments",
                column: "comic_chapter_id");

            migrationBuilder.CreateIndex(
                name: "IX_comic_comments_comic_id",
                table: "comic_comments",
                column: "comic_id");

            migrationBuilder.CreateIndex(
                name: "IX_comic_comments_id",
                table: "comic_comments",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_comic_comments_reply_id",
                table: "comic_comments",
                column: "reply_id");

            migrationBuilder.CreateIndex(
                name: "IX_comic_comments_user_id",
                table: "comic_comments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_comic_have_categories_comic_category_id",
                table: "comic_have_categories",
                column: "comic_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_comic_have_categories_comic_id",
                table: "comic_have_categories",
                column: "comic_id");

            migrationBuilder.CreateIndex(
                name: "IX_comics_id",
                table: "comics",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_comics_slug",
                table: "comics",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_id",
                table: "RefreshTokens",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_token",
                table: "RefreshTokens",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_user_id",
                table: "RefreshTokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_has_roles_assigned_by",
                table: "user_has_roles",
                column: "assigned_by");

            migrationBuilder.CreateIndex(
                name: "IX_user_has_roles_id",
                table: "user_has_roles",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_user_has_roles_user_id",
                table: "user_has_roles",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_email",
                table: "Users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_id",
                table: "Users",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comic_comments");

            migrationBuilder.DropTable(
                name: "comic_have_categories");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "user_has_roles");

            migrationBuilder.DropTable(
                name: "comic_chapters");

            migrationBuilder.DropTable(
                name: "comic_categories");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "comics");
        }
    }
}
