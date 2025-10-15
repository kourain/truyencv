using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TruyenCV.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryTypeToComicCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "category_type",
                table: "comic_categories",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.InsertData(
                table: "user_has_roles",
                columns: new[] { "id", "assigned_by", "created_at", "deleted_at", "revoked_at", "role_name", "updated_at", "user_id" },
                values: new object[] { 766206486593409025L, 1L, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "System", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), 1L });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 1L,
                column: "password",
                value: "$2a$12$4mDXXIMuUV8.QstZN1crHeml7Wbhen1O6QvJ6bXDMJ8z2dHxUnDYi");

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "avatar", "banned_at", "bookmark_count", "coin", "created_at", "deleted_at", "email", "email_verified_at", "is_banned", "key", "name", "password", "phone", "read_chapter_count", "read_comic_count", "updated_at" },
                values: new object[] { 766206485104431104L, "default_avatar.png", null, 0L, 0L, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "maiquyen16503@gmail.com", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), false, 0L, "kourain", "$2a$12$Gws/QykY/zHKBnzvvb8lGuFtxmfMPFK0EV9ECV7LBzrQKfA.EiTIe", "0123456789", 0L, 0L, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "user_has_roles",
                columns: new[] { "id", "assigned_by", "created_at", "deleted_at", "revoked_at", "role_name", "updated_at", "user_id" },
                values: new object[,]
                {
                    { 766206486589214720L, 1L, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Admin", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), 766206485104431104L },
                    { 766206486593409024L, 1L, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "User", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), 766206485104431104L }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "user_has_roles",
                keyColumn: "id",
                keyValue: 766206486589214720L);

            migrationBuilder.DeleteData(
                table: "user_has_roles",
                keyColumn: "id",
                keyValue: 766206486593409024L);

            migrationBuilder.DeleteData(
                table: "user_has_roles",
                keyColumn: "id",
                keyValue: 766206486593409025L);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: 766206485104431104L);

            migrationBuilder.DropColumn(
                name: "category_type",
                table: "comic_categories");

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
    }
}
