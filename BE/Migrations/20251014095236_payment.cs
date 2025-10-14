using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TruyenCV.Migrations
{
    /// <inheritdoc />
    public partial class payment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "user_has_roles",
                keyColumn: "id",
                keyValue: 765777152539889664L);

            migrationBuilder.DeleteData(
                table: "user_has_roles",
                keyColumn: "id",
                keyValue: 765777152539889665L);

            migrationBuilder.DeleteData(
                table: "user_has_roles",
                keyColumn: "id",
                keyValue: 765777152539889666L);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: 765777151059300352L);

            migrationBuilder.AddColumn<long>(
                name: "key",
                table: "users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "key_require",
                table: "comic_chapters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "key_require_until",
                table: "comic_chapters",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "payment_history",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    amount_coin = table.Column<long>(type: "bigint", nullable: false),
                    amount_money = table.Column<long>(type: "bigint", nullable: false),
                    payment_method = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    reference_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_payment_history_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "subscriptions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    price_coin = table.Column<long>(type: "bigint", nullable: false),
                    duration_day = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscriptions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_coin_history",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    coin = table.Column<long>(type: "bigint", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    reason = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    reference_id = table.Column<long>(type: "bigint", nullable: true),
                    reference_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_coin_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_coin_history_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_use_key_history",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    key = table.Column<long>(type: "bigint", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    chapter_id = table.Column<long>(type: "bigint", nullable: true),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_use_key_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_use_key_history_comic_chapters_chapter_id",
                        column: x => x.chapter_id,
                        principalTable: "comic_chapters",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_user_use_key_history_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_has_subscriptions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    subscription_id = table.Column<long>(type: "bigint", nullable: false),
                    start_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    auto_renew = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_has_subscriptions", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_has_subscriptions_subscriptions_subscription_id",
                        column: x => x.subscription_id,
                        principalTable: "subscriptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_has_subscriptions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "user_has_roles",
                columns: new[] { "id", "assigned_by", "created_at", "deleted_at", "role_name", "updated_at", "user_id" },
                values: new object[] { 765874687442948098L, 1L, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "System", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), 1L });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 1L,
                columns: new[] { "key", "password" },
                values: new object[] { 0L, "$2a$12$3x2c5N6mdEHMQgPuK8ojpuREA6t1cl8P6AIt/FJwSlkGS5tcNHg2y" });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "avatar", "banned_at", "bookmark_count", "coin", "created_at", "deleted_at", "email", "email_verified_at", "is_banned", "key", "name", "password", "phone", "read_chapter_count", "read_comic_count", "updated_at" },
                values: new object[] { 765874686000107520L, "default_avatar.png", null, 0L, 0L, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "maiquyen16503@gmail.com", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), false, 0L, "kourain", "$2a$12$z/ebanm/q31uoDWsw5M4dOygNalcuuM/Jf2R54nfNCPe06fJRvySy", "0123456789", 0L, 0L, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "user_has_roles",
                columns: new[] { "id", "assigned_by", "created_at", "deleted_at", "role_name", "updated_at", "user_id" },
                values: new object[,]
                {
                    { 765874687442948096L, 1L, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Admin", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), 765874686000107520L },
                    { 765874687442948097L, 1L, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "User", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), 765874686000107520L }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentHistory_User",
                table: "payment_history",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_code",
                table: "subscriptions",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserCoinHistory_User",
                table: "user_coin_history",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_has_subscriptions_subscription_id",
                table: "user_has_subscriptions",
                column: "subscription_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_has_subscriptions_user_id_subscription_id",
                table: "user_has_subscriptions",
                columns: new[] { "user_id", "subscription_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_use_key_history_chapter_id",
                table: "user_use_key_history",
                column: "chapter_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserUseKeyHistory_User",
                table: "user_use_key_history",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "payment_history");

            migrationBuilder.DropTable(
                name: "user_coin_history");

            migrationBuilder.DropTable(
                name: "user_has_subscriptions");

            migrationBuilder.DropTable(
                name: "user_use_key_history");

            migrationBuilder.DropTable(
                name: "subscriptions");

            migrationBuilder.DeleteData(
                table: "user_has_roles",
                keyColumn: "id",
                keyValue: 765874687442948096L);

            migrationBuilder.DeleteData(
                table: "user_has_roles",
                keyColumn: "id",
                keyValue: 765874687442948097L);

            migrationBuilder.DeleteData(
                table: "user_has_roles",
                keyColumn: "id",
                keyValue: 765874687442948098L);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: 765874686000107520L);

            migrationBuilder.DropColumn(
                name: "key",
                table: "users");

            migrationBuilder.DropColumn(
                name: "key_require",
                table: "comic_chapters");

            migrationBuilder.DropColumn(
                name: "key_require_until",
                table: "comic_chapters");

            migrationBuilder.InsertData(
                table: "user_has_roles",
                columns: new[] { "id", "assigned_by", "created_at", "deleted_at", "role_name", "updated_at", "user_id" },
                values: new object[] { 765777152539889666L, 1L, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "System", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), 1L });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 1L,
                column: "password",
                value: "$2a$12$IX70SAFiyBMxIKYeafybceq8mGOzRC8gKtQ4rLPgop//2soyYuOsy");

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "avatar", "banned_at", "bookmark_count", "coin", "created_at", "deleted_at", "email", "email_verified_at", "is_banned", "name", "password", "phone", "read_chapter_count", "read_comic_count", "updated_at" },
                values: new object[] { 765777151059300352L, "default_avatar.png", null, 0L, 0L, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "maiquyen16503@gmail.com", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), false, "kourain", "$2a$12$TMBz3RFBpSJ1bAcsPHUAhubOL9faDXiUAZYmDnNqKRZEzRE7MU55C", "0123456789", 0L, 0L, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "user_has_roles",
                columns: new[] { "id", "assigned_by", "created_at", "deleted_at", "role_name", "updated_at", "user_id" },
                values: new object[,]
                {
                    { 765777152539889664L, 1L, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Admin", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), 765777151059300352L },
                    { 765777152539889665L, 1L, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "User", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), 765777151059300352L }
                });
        }
    }
}
