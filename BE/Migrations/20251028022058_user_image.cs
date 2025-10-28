using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TruyenCV.Migrations
{
    /// <inheritdoc />
    public partial class user_image : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "avatar",
                table: "users",
                type: "character varying(15360)",
                maxLength: 15360,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "avatar",
                table: "users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(15360)",
                oldMaxLength: 15360);
        }
    }
}
