using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TruyenCV.Migrations
{
    /// <inheritdoc />
    public partial class constrantcheck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_coin_history_users_user_id",
                table: "user_coin_history");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_coin_history",
                table: "user_coin_history");

            migrationBuilder.RenameTable(
                name: "user_coin_history",
                newName: "user_use_coin_history");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_use_coin_history",
                table: "user_use_coin_history",
                column: "id");

            migrationBuilder.InsertData(
                table: "comic_categories",
                columns: new[] { "id", "category_type", "created_at", "deleted_at", "name", "updated_at" },
                values: new object[,]
                {
                    { 1001L, 2, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Tiên Hiệp", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 1002L, 2, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Huyền Huyễn", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 1003L, 2, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Khoa Huyễn", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 1004L, 2, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Võng Du", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 1005L, 2, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Đô Thị", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 1006L, 2, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Đồng Nhân", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 1007L, 2, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Dã Sử", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 1008L, 2, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cạnh Kỹ", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 1009L, 2, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Huyền Nghi", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 1010L, 2, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Kiếm Hiệp", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 1011L, 2, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Kỳ Ảo", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 1012L, 2, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Light Novel", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2001L, 4, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Điềm Đạm", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2002L, 4, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Nhiệt Huyết", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2003L, 4, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Vô Sỉ", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2004L, 4, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thiết Huyết", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2005L, 4, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Nhẹ Nhàng", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2006L, 4, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cơ Trí", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2007L, 4, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Lãnh Khốc", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2008L, 4, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Kiêu Ngạo", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2009L, 4, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ngu Ngốc", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2010L, 4, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Giảo Hoạt", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3001L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Đông Phương Huyền Huyễn", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3002L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Dị Thế Đại Lục", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3003L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Vương Triều Tranh Bá", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3004L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cao Võ Thế Giới", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3005L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Tây Phương Kỳ Huyễn", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3006L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Hiện Đại Ma Pháp", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3007L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Hắc Ám Huyễn Tưởng", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3008L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Lịch Sử Thần Thoại", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3009L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Võ Hiệp Huyễn Tưởng", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3010L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cổ Võ Tương Lai", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3011L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Tu Chân Văn Minh", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3012L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Huyễn Tưởng Tu Tiên", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3013L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Hiện Đại Tu Chân", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3014L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thần Thoại Tu Chân", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3015L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cổ Điển Tiên Hiệp", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3016L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Viễn Cổ Hồng Hoang", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3017L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Đô Thị Sinh Hoạt", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3018L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Đô Thị Dị Năng", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3019L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thanh Xuân Vườn Trường", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3020L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ngu Nhạc Minh Tinh", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3021L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thương Chiến Chức Tràng", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3022L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Giá Không Lịch Sử", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3023L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Lịch Sử Quân Sự", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3024L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Dân Gian Truyền Thuyết", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3025L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Lịch Sử Quan Trường", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3026L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Hư Nghĩ Võng Du", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3027L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Du Hí Dị Giới", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3028L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Điện Tử Cạnh Kỹ", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3029L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thể Dục Cạnh Kỹ", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3030L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cổ Võ Cơ Giáp", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3031L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thế Giới Tương Lai", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3032L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Tinh Tế Văn Minh", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3033L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Tiến Hóa Biến Dị", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3034L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Mạt Thế Nguy Cơ", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3035L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thời Không Xuyên Toa", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3036L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Quỷ Bí Huyền Nghi", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3037L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Kỳ Diệu Thế Giới", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3038L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Trinh Tham Thôi Lý", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3039L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thám Hiểm Sinh Tồn", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3040L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cung Vi Trạch Đấu", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3041L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Kinh Thương Chủng Điền", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3042L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Tiên Lữ Kỳ Duyên", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3043L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Hào Môn Thế Gia", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3044L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Dị Tộc Luyến Tình", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3045L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ma Pháp Huyễn Tình", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3046L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Tinh Tế Luyến Ca", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3047L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Linh Khí Khôi Phục", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3048L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Chư Thiên Vạn Giới", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3049L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Nguyên Sinh Huyễn Tưởng", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3050L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Yêu Đương Thường Ngày", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3051L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Diễn Sinh Đồng Nhân", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3052L, 3, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cáo Tiếu Thổ Tào", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4001L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Hệ Thống", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4002L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Lão Gia", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4003L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Bàn Thờ", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4004L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Tùy Thân", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4005L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Phàm Nhân", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4006L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Vô Địch", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4007L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Xuyên Qua", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4008L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Nữ Cường", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4009L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Khế Ước", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4010L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Trọng Sinh", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4011L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Hồng Lâu", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4012L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Học Viện", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4013L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Biến Thân", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4014L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cổ Ngu", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4015L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Chuyển Thế", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4016L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Xuyên Sách", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4017L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Đàn Xuyên", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4018L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Phế Tài", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4019L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Dưỡng Thành", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4020L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cơm Mềm", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4021L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Vô Hạn", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4022L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Mary Sue", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4023L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cá Mặn", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4024L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Xây Dựng Thế Lực", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4025L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Xuyên Nhanh", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4026L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Nữ Phụ", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4027L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Vả Mặt", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4028L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Sảng Văn", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4029L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Xuyên Không", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4030L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ngọt Sủng", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4031L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Ngự Thú", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4032L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Điền Viên", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4033L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Toàn Dân", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4034L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Mỹ Thực", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4035L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Phản Phái", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4036L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Sau Màn", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4037L, 5, new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Thiên Tài", new DateTime(2025, 9, 23, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_User_bookmark_count_Positive",
                table: "users",
                sql: "bookmark_count >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_User_coin_Positive",
                table: "users",
                sql: "coin >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_User_key_Positive",
                table: "users",
                sql: "key >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_User_read_chapter_count_Positive",
                table: "users",
                sql: "read_chapter_count >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_User_read_comic_count_Positive",
                table: "users",
                sql: "read_comic_count >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Comic_bookmark_count_Positive",
                table: "comics",
                sql: "bookmark_count >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Comic_chapter_count_Positive",
                table: "comics",
                sql: "chapter_count >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_comic_comments_rate_star_range",
                table: "comic_comments",
                sql: "(rate_star IS NULL) OR (rate_star BETWEEN 1 AND 5)");

            migrationBuilder.AddForeignKey(
                name: "FK_user_use_coin_history_users_user_id",
                table: "user_use_coin_history",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_use_coin_history_users_user_id",
                table: "user_use_coin_history");

            migrationBuilder.DropCheckConstraint(
                name: "CK_User_bookmark_count_Positive",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CK_User_coin_Positive",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CK_User_key_Positive",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CK_User_read_chapter_count_Positive",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CK_User_read_comic_count_Positive",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Comic_bookmark_count_Positive",
                table: "comics");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Comic_chapter_count_Positive",
                table: "comics");

            migrationBuilder.DropCheckConstraint(
                name: "CK_comic_comments_rate_star_range",
                table: "comic_comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_use_coin_history",
                table: "user_use_coin_history");

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1001L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1002L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1003L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1004L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1005L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1006L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1007L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1008L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1009L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1010L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1011L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 1012L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 2001L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 2002L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 2003L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 2004L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 2005L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 2006L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 2007L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 2008L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 2009L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 2010L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3001L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3002L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3003L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3004L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3005L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3006L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3007L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3008L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3009L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3010L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3011L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3012L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3013L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3014L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3015L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3016L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3017L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3018L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3019L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3020L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3021L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3022L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3023L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3024L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3025L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3026L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3027L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3028L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3029L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3030L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3031L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3032L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3033L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3034L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3035L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3036L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3037L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3038L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3039L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3040L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3041L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3042L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3043L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3044L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3045L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3046L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3047L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3048L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3049L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3050L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3051L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 3052L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4001L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4002L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4003L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4004L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4005L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4006L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4007L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4008L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4009L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4010L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4011L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4012L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4013L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4014L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4015L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4016L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4017L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4018L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4019L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4020L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4021L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4022L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4023L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4024L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4025L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4026L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4027L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4028L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4029L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4030L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4031L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4032L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4033L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4034L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4035L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4036L);

            migrationBuilder.DeleteData(
                table: "comic_categories",
                keyColumn: "id",
                keyValue: 4037L);

            migrationBuilder.RenameTable(
                name: "user_use_coin_history",
                newName: "user_coin_history");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_coin_history",
                table: "user_coin_history",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_user_coin_history_users_user_id",
                table: "user_coin_history",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
