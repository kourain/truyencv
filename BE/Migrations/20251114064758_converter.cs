using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TruyenCV.Migrations
{
    /// <inheritdoc />
    public partial class converter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "user_has_roles",
                columns: new[] { "id", "assigned_by", "created_at", "deleted_at", "revoked_at", "role_name", "updated_at", "user_id" },
                values: new object[] { 766206486593409021L, 1L, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Converter", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), 766206485104431104L });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "user_has_roles",
                keyColumn: "id",
                keyValue: 766206486593409021L);
        }
    }
}
