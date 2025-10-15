using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TruyenCV.Migrations
{
    /// <inheritdoc />
    public partial class add_revoke_fields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<DateTime>(
                name: "revoked_at",
                table: "user_has_roles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "revoke_until",
                table: "user_has_permissions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "revoked_at",
                table: "user_has_permissions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.InsertData(
                table: "user_has_roles",
                columns: new[] { "id", "assigned_by", "created_at", "deleted_at", "revoked_at", "role_name", "updated_at", "user_id" },
                values: new object[] { 766130041049321473L, 1L, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "System", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), 1L });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 1L,
                column: "password",
                value: "$2a$12$3YxsjLQBcitvIFdaKsRafOjObtBwZ/Zp11YAm.cMIUC77.GiaFRIi");

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "avatar", "banned_at", "bookmark_count", "coin", "created_at", "deleted_at", "email", "email_verified_at", "is_banned", "key", "name", "password", "phone", "read_chapter_count", "read_comic_count", "updated_at" },
                values: new object[] { 766130039581315072L, "default_avatar.png", null, 0L, 0L, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "maiquyen16503@gmail.com", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), false, 0L, "kourain", "$2a$12$lLwkSfSmAN651GucYOizkejIYkKz5kNgmBu.I72gWk3EQTXl7k0i2", "0123456789", 0L, 0L, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "user_has_roles",
                columns: new[] { "id", "assigned_by", "created_at", "deleted_at", "revoked_at", "role_name", "updated_at", "user_id" },
                values: new object[,]
                {
                    { 766130041045127168L, 1L, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Admin", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), 766130039581315072L },
                    { 766130041049321472L, 1L, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "User", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), 766130039581315072L }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "user_has_roles",
                keyColumn: "id",
                keyValue: 766130041045127168L);

            migrationBuilder.DeleteData(
                table: "user_has_roles",
                keyColumn: "id",
                keyValue: 766130041049321472L);

            migrationBuilder.DeleteData(
                table: "user_has_roles",
                keyColumn: "id",
                keyValue: 766130041049321473L);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: 766130039581315072L);

            migrationBuilder.DropColumn(
                name: "revoked_at",
                table: "user_has_roles");

            migrationBuilder.DropColumn(
                name: "revoke_until",
                table: "user_has_permissions");

            migrationBuilder.DropColumn(
                name: "revoked_at",
                table: "user_has_permissions");

            migrationBuilder.InsertData(
                table: "user_has_roles",
                columns: new[] { "id", "assigned_by", "created_at", "deleted_at", "role_name", "updated_at", "user_id" },
                values: new object[] { 765874687442948098L, 1L, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "System", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), 1L });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 1L,
                column: "password",
                value: "$2a$12$3x2c5N6mdEHMQgPuK8ojpuREA6t1cl8P6AIt/FJwSlkGS5tcNHg2y");

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
        }
    }
}
