using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TruyenCV.Migrations
{
    /// <inheritdoc />
    public partial class disableautoinclude : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comics_users_embedded_by",
                table: "comics");

            migrationBuilder.AddColumn<DateTime>(
                name: "accept_at",
                table: "comics",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "accept_by",
                table: "comics",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "author_slug",
                table: "comics",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "main_category_id",
                table: "comics",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "user_comic_recommends",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    comic_id = table.Column<long>(type: "bigint", nullable: false),
                    month = table.Column<int>(type: "integer", nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_comic_recommends", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_comic_recommends_comics_comic_id",
                        column: x => x.comic_id,
                        principalTable: "comics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_comic_recommends_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1001L,
                column: "category_type",
                value: 1);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1002L,
                column: "category_type",
                value: 1);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1003L,
                column: "category_type",
                value: 1);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1004L,
                column: "category_type",
                value: 1);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1005L,
                column: "category_type",
                value: 1);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1006L,
                column: "category_type",
                value: 1);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1007L,
                column: "category_type",
                value: 1);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1008L,
                column: "category_type",
                value: 1);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1009L,
                column: "category_type",
                value: 1);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1010L,
                column: "category_type",
                value: 1);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1011L,
                column: "category_type",
                value: 1);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1012L,
                column: "category_type",
                value: 1);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 2001L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 2002L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 2003L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 2004L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 2005L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 2006L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 2007L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 2008L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 2009L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 2010L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3001L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3002L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3003L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3004L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3005L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3006L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3007L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3008L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3009L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3010L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3011L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3012L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3013L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3014L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3015L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3016L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3017L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3018L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3019L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3020L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3021L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3022L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3023L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3024L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3025L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3026L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3027L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3028L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3029L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3030L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3031L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3032L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3033L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3034L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3035L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3036L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3037L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3038L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3039L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3040L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3041L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3042L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3043L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3044L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3045L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3046L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3047L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3048L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3049L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3050L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3051L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3052L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4001L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4002L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4003L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4004L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4005L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4006L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4007L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4008L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4009L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4010L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4011L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4012L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4013L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4014L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4015L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4016L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4017L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4018L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4019L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4020L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4021L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4022L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4023L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4024L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4025L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4026L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4027L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4028L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4029L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4030L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4031L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4032L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4033L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4034L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4035L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4036L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4037L,
                column: "category_type",
                value: 4);

            migrationBuilder.InsertData(
                table: "comic_categories",
                columns: new[] { "id", "category_type", "created_at", "deleted_at", "name", "updated_at" },
                values: new object[,]
                {
                    { 4038L, 4, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Trò Chơi", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5001L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Góc Nhìn Nam", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5002L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Góc Nhìn Nữ", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_comics_accept_by",
                table: "comics",
                column: "accept_by");

            migrationBuilder.CreateIndex(
                name: "IX_comics_main_category_id",
                table: "comics",
                column: "main_category_id");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Comic_main_category_id_Valid",
                table: "comics",
                sql: "main_category_id < 2000");

            migrationBuilder.AddCheckConstraint(
                name: "CK_comic_have_categories_comic_category_id",
                table: "comic_have_categories",
                sql: "comic_category_id > 2000");

            migrationBuilder.CreateIndex(
                name: "IX_user_comic_recommends_comic_id",
                table: "user_comic_recommends",
                column: "comic_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_comic_recommends_user_id_month_year",
                table: "user_comic_recommends",
                columns: new[] { "user_id", "month", "year" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_comics_comic_categories_main_category_id",
                table: "comics",
                column: "main_category_id",
                principalTable: "comic_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_comics_users_accept_by",
                table: "comics",
                column: "accept_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_comics_users_embedded_by",
                table: "comics",
                column: "embedded_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comics_comic_categories_main_category_id",
                table: "comics");

            migrationBuilder.DropForeignKey(
                name: "FK_comics_users_accept_by",
                table: "comics");

            migrationBuilder.DropForeignKey(
                name: "FK_comics_users_embedded_by",
                table: "comics");

            migrationBuilder.DropTable(
                name: "user_comic_recommends");

            migrationBuilder.DropIndex(
                name: "IX_comics_accept_by",
                table: "comics");

            migrationBuilder.DropIndex(
                name: "IX_comics_main_category_id",
                table: "comics");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Comic_main_category_id_Valid",
                table: "comics");

            migrationBuilder.DropCheckConstraint(
                name: "CK_comic_have_categories_comic_category_id",
                table: "comic_have_categories");

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4038L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 5001L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 5002L);

            migrationBuilder.DropColumn(
                name: "accept_at",
                table: "comics");

            migrationBuilder.DropColumn(
                name: "accept_by",
                table: "comics");

            migrationBuilder.DropColumn(
                name: "author_slug",
                table: "comics");

            migrationBuilder.DropColumn(
                name: "main_category_id",
                table: "comics");

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1001L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1002L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1003L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1004L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1005L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1006L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1007L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1008L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1009L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1010L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1011L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1012L,
                column: "category_type",
                value: 2);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 2001L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 2002L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 2003L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 2004L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 2005L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 2006L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 2007L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 2008L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 2009L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 2010L,
                column: "category_type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3001L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3002L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3003L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3004L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3005L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3006L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3007L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3008L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3009L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3010L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3011L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3012L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3013L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3014L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3015L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3016L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3017L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3018L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3019L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3020L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3021L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3022L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3023L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3024L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3025L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3026L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3027L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3028L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3029L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3030L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3031L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3032L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3033L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3034L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3035L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3036L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3037L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3038L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3039L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3040L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3041L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3042L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3043L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3044L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3045L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3046L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3047L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3048L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3049L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3050L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3051L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3052L,
                column: "category_type",
                value: 3);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4001L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4002L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4003L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4004L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4005L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4006L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4007L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4008L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4009L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4010L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4011L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4012L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4013L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4014L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4015L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4016L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4017L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4018L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4019L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4020L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4021L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4022L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4023L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4024L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4025L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4026L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4027L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4028L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4029L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4030L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4031L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4032L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4033L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4034L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4035L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4036L,
                column: "category_type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4037L,
                column: "category_type",
                value: 5);

            migrationBuilder.AddForeignKey(
                name: "FK_comics_users_embedded_by",
                table: "comics",
                column: "embedded_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
