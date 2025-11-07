using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace TruyenCV.Migrations
{
    /// <inheritdoc />
    public partial class comiccomment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ticket_added",
                table: "subscriptions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Vector>(
                name: "search_vector",
                table: "comics",
                type: "vector(768)",
                nullable: true,
                oldClrType: typeof(Vector),
                oldType: "vector(256)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_hidden",
                table: "comic_comments",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ticket_added",
                table: "subscriptions");

            migrationBuilder.DropColumn(
                name: "is_hidden",
                table: "comic_comments");

            migrationBuilder.AlterColumn<Vector>(
                name: "search_vector",
                table: "comics",
                type: "vector(256)",
                nullable: true,
                oldClrType: typeof(Vector),
                oldType: "vector(768)",
                oldNullable: true);
        }
    }
}
