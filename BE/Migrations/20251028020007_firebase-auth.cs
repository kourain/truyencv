using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TruyenCV.Migrations
{
    /// <inheritdoc />
    public partial class firebaseauth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "firebase_uid",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 1L,
                column: "firebase_uid",
                value: null);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: 766206485104431104L,
                column: "firebase_uid",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_users_firebase_uid",
                table: "users",
                column: "firebase_uid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_firebase_uid",
                table: "users");

            migrationBuilder.DropColumn(
                name: "firebase_uid",
                table: "users");
        }
    }
}
