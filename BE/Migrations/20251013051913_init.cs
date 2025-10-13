using System;
using Microsoft.EntityFrameworkCore.Migrations;

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
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
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
                name: "users",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    password = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    phone = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    email_verified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    read_comic_count = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    read_chapter_count = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    bookmark_count = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    coin = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    is_banned = table.Column<bool>(type: "boolean", nullable: false),
                    banned_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    avatar = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "comics",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    slug = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    author = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    embedded_from = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    embedded_from_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    embedded_by = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    chapter_count = table.Column<long>(type: "bigint", nullable: false),
                    bookmark_count = table.Column<long>(type: "bigint", nullable: false),
                    published_year = table.Column<long>(type: "bigint", nullable: true),
                    rate = table.Column<float>(type: "real", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comics", x => x.id);
                    table.ForeignKey(
                        name: "FK_comics_users_embedded_by",
                        column: x => x.embedded_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    token = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_has_permissions",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    permissions = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    assigned_by = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_has_permissions", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_has_permissions_users_assigned_by",
                        column: x => x.assigned_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_user_has_permissions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_has_roles",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    role_name = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    assigned_by = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_has_roles", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_has_roles_users_assigned_by",
                        column: x => x.assigned_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_user_has_roles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comic_chapters",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    comic_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
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
                    comic_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    comic_category_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
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
                name: "user_comic_bookmarks",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    user_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    comic_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_comic_bookmarks", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_comic_bookmarks_comics_comic_id",
                        column: x => x.comic_id,
                        principalTable: "comics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_comic_bookmarks_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_comic_read_history",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    user_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    comic_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    chapter_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_comic_read_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_comic_read_history_comics_comic_id",
                        column: x => x.comic_id,
                        principalTable: "comics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_comic_read_history_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comic_comments",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    comic_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    comic_chapter_id = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    user_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: false),
                    like = table.Column<int>(type: "integer", nullable: false),
                    reply_id = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
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
                        name: "FK_comic_comments_comic_chapters_comic_chapter_id",
                        column: x => x.comic_chapter_id,
                        principalTable: "comic_chapters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
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
                    table.ForeignKey(
                        name: "FK_comic_comments_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "avatar", "banned_at", "bookmark_count", "coin", "created_at", "deleted_at", "email", "email_verified_at", "is_banned", "name", "password", "phone", "read_chapter_count", "read_comic_count", "updated_at" },
                values: new object[,]
                {
                    { 1m, "default_avatar.png", null, 0m, 0m, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "ht.kourain@gmail.com", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), false, "System", "$2a$12$0jPXKmQRxJHEGeuDoXrD7uKVfPk7MGISLXPmrSz62gKbBw1NDdaLC", "0000000000", 0m, 0m, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 765443497779007488m, "default_avatar.png", null, 0m, 0m, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "maiquyen16503@gmail.com", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), false, "kourain", "$2a$12$95O7eg4xl8Y8shqLTXgXH.zRvlpI8dITVTd8BHvojT04I8DPcNn6a", "0123456789", 0m, 0m, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "user_has_roles",
                columns: new[] { "id", "assigned_by", "created_at", "deleted_at", "role_name", "updated_at", "user_id" },
                values: new object[,]
                {
                    { 765443499280568320m, 1m, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Admin", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), 765443497779007488m },
                    { 765443499284762624m, 1m, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "User", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), 765443497779007488m },
                    { 765443499284762625m, 1m, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "System", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), 1m }
                });

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
                name: "IX_comic_comments_comic_chapter_id",
                table: "comic_comments",
                column: "comic_chapter_id");

            migrationBuilder.CreateIndex(
                name: "IX_comic_comments_comic_id",
                table: "comic_comments",
                column: "comic_id");

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
                name: "IX_comics_embedded_by",
                table: "comics",
                column: "embedded_by");

            migrationBuilder.CreateIndex(
                name: "IX_comics_slug",
                table: "comics",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_token",
                table: "refresh_tokens",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_user_id",
                table: "refresh_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_comic_bookmarks_comic_id",
                table: "user_comic_bookmarks",
                column: "comic_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_comic_bookmarks_user_id_comic_id",
                table: "user_comic_bookmarks",
                columns: new[] { "user_id", "comic_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserComicBookmark",
                table: "user_comic_bookmarks",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_comic_read_history_comic_id",
                table: "user_comic_read_history",
                column: "comic_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_comic_read_history_user_id_comic_id",
                table: "user_comic_read_history",
                columns: new[] { "user_id", "comic_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserComicReadHistory",
                table: "user_comic_read_history",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_has_permissions_assigned_by",
                table: "user_has_permissions",
                column: "assigned_by");

            migrationBuilder.CreateIndex(
                name: "IX_UserHasPermission",
                table: "user_has_permissions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_has_roles_assigned_by",
                table: "user_has_roles",
                column: "assigned_by");

            migrationBuilder.CreateIndex(
                name: "IX_UserHasRole",
                table: "user_has_roles",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comic_comments");

            migrationBuilder.DropTable(
                name: "comic_have_categories");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "user_comic_bookmarks");

            migrationBuilder.DropTable(
                name: "user_comic_read_history");

            migrationBuilder.DropTable(
                name: "user_has_permissions");

            migrationBuilder.DropTable(
                name: "user_has_roles");

            migrationBuilder.DropTable(
                name: "comic_chapters");

            migrationBuilder.DropTable(
                name: "comic_categories");

            migrationBuilder.DropTable(
                name: "comics");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
