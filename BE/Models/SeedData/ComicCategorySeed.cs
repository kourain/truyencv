using System;
using Microsoft.EntityFrameworkCore;
using TruyenCV.Models;

namespace TruyenCV.Models.SeedData;

public static class ComicCategorySeed
{
    public static void Seed(ModelBuilder modelBuilder, DateTime defaultDate)
    {
        ComicCategory CreateCategory(long id, string name, CategoryType type) => new()
        {
            id = id,
            name = name,
            category_type = type,
            created_at = defaultDate,
            updated_at = defaultDate
        };

        var comicCategories = new[]
        {
            // Genres
            CreateCategory(1001, "Tiên Hiệp", CategoryType.Genre),
            CreateCategory(1002, "Huyền Huyễn", CategoryType.Genre),
            CreateCategory(1003, "Khoa Huyễn", CategoryType.Genre),
            CreateCategory(1004, "Võng Du", CategoryType.Genre),
            CreateCategory(1005, "Đô Thị", CategoryType.Genre),
            CreateCategory(1006, "Đồng Nhân", CategoryType.Genre),
            CreateCategory(1007, "Dã Sử", CategoryType.Genre),
            CreateCategory(1008, "Cạnh Kỹ", CategoryType.Genre),
            CreateCategory(1009, "Huyền Nghi", CategoryType.Genre),
            CreateCategory(1010, "Kiếm Hiệp", CategoryType.Genre),
            CreateCategory(1011, "Kỳ Ảo", CategoryType.Genre),
            CreateCategory(1012, "Light Novel", CategoryType.Genre),

            // Main character traits
            CreateCategory(2001, "Điềm Đạm", CategoryType.MainCharacter),
            CreateCategory(2002, "Nhiệt Huyết", CategoryType.MainCharacter),
            CreateCategory(2003, "Vô Sỉ", CategoryType.MainCharacter),
            CreateCategory(2004, "Thiết Huyết", CategoryType.MainCharacter),
            CreateCategory(2005, "Nhẹ Nhàng", CategoryType.MainCharacter),
            CreateCategory(2006, "Cơ Trí", CategoryType.MainCharacter),
            CreateCategory(2007, "Lãnh Khốc", CategoryType.MainCharacter),
            CreateCategory(2008, "Kiêu Ngạo", CategoryType.MainCharacter),
            CreateCategory(2009, "Ngu Ngốc", CategoryType.MainCharacter),
            CreateCategory(2010, "Giảo Hoạt", CategoryType.MainCharacter),

            // World themes
            CreateCategory(3001, "Đông Phương Huyền Huyễn", CategoryType.WorldTheme),
            CreateCategory(3002, "Dị Thế Đại Lục", CategoryType.WorldTheme),
            CreateCategory(3003, "Vương Triều Tranh Bá", CategoryType.WorldTheme),
            CreateCategory(3004, "Cao Võ Thế Giới", CategoryType.WorldTheme),
            CreateCategory(3005, "Tây Phương Kỳ Huyễn", CategoryType.WorldTheme),
            CreateCategory(3006, "Hiện Đại Ma Pháp", CategoryType.WorldTheme),
            CreateCategory(3007, "Hắc Ám Huyễn Tưởng", CategoryType.WorldTheme),
            CreateCategory(3008, "Lịch Sử Thần Thoại", CategoryType.WorldTheme),
            CreateCategory(3009, "Võ Hiệp Huyễn Tưởng", CategoryType.WorldTheme),
            CreateCategory(3010, "Cổ Võ Tương Lai", CategoryType.WorldTheme),
            CreateCategory(3011, "Tu Chân Văn Minh", CategoryType.WorldTheme),
            CreateCategory(3012, "Huyễn Tưởng Tu Tiên", CategoryType.WorldTheme),
            CreateCategory(3013, "Hiện Đại Tu Chân", CategoryType.WorldTheme),
            CreateCategory(3014, "Thần Thoại Tu Chân", CategoryType.WorldTheme),
            CreateCategory(3015, "Cổ Điển Tiên Hiệp", CategoryType.WorldTheme),
            CreateCategory(3016, "Viễn Cổ Hồng Hoang", CategoryType.WorldTheme),
            CreateCategory(3017, "Đô Thị Sinh Hoạt", CategoryType.WorldTheme),
            CreateCategory(3018, "Đô Thị Dị Năng", CategoryType.WorldTheme),
            CreateCategory(3019, "Thanh Xuân Vườn Trường", CategoryType.WorldTheme),
            CreateCategory(3020, "Ngu Nhạc Minh Tinh", CategoryType.WorldTheme),
            CreateCategory(3021, "Thương Chiến Chức Tràng", CategoryType.WorldTheme),
            CreateCategory(3022, "Giá Không Lịch Sử", CategoryType.WorldTheme),
            CreateCategory(3023, "Lịch Sử Quân Sự", CategoryType.WorldTheme),
            CreateCategory(3024, "Dân Gian Truyền Thuyết", CategoryType.WorldTheme),
            CreateCategory(3025, "Lịch Sử Quan Trường", CategoryType.WorldTheme),
            CreateCategory(3026, "Hư Nghĩ Võng Du", CategoryType.WorldTheme),
            CreateCategory(3027, "Du Hí Dị Giới", CategoryType.WorldTheme),
            CreateCategory(3028, "Điện Tử Cạnh Kỹ", CategoryType.WorldTheme),
            CreateCategory(3029, "Thể Dục Cạnh Kỹ", CategoryType.WorldTheme),
            CreateCategory(3030, "Cổ Võ Cơ Giáp", CategoryType.WorldTheme),
            CreateCategory(3031, "Thế Giới Tương Lai", CategoryType.WorldTheme),
            CreateCategory(3032, "Tinh Tế Văn Minh", CategoryType.WorldTheme),
            CreateCategory(3033, "Tiến Hóa Biến Dị", CategoryType.WorldTheme),
            CreateCategory(3034, "Mạt Thế Nguy Cơ", CategoryType.WorldTheme),
            CreateCategory(3035, "Thời Không Xuyên Toa", CategoryType.WorldTheme),
            CreateCategory(3036, "Quỷ Bí Huyền Nghi", CategoryType.WorldTheme),
            CreateCategory(3037, "Kỳ Diệu Thế Giới", CategoryType.WorldTheme),
            CreateCategory(3038, "Trinh Tham Thôi Lý", CategoryType.WorldTheme),
            CreateCategory(3039, "Thám Hiểm Sinh Tồn", CategoryType.WorldTheme),
            CreateCategory(3040, "Cung Vi Trạch Đấu", CategoryType.WorldTheme),
            CreateCategory(3041, "Kinh Thương Chủng Điền", CategoryType.WorldTheme),
            CreateCategory(3042, "Tiên Lữ Kỳ Duyên", CategoryType.WorldTheme),
            CreateCategory(3043, "Hào Môn Thế Gia", CategoryType.WorldTheme),
            CreateCategory(3044, "Dị Tộc Luyến Tình", CategoryType.WorldTheme),
            CreateCategory(3045, "Ma Pháp Huyễn Tình", CategoryType.WorldTheme),
            CreateCategory(3046, "Tinh Tế Luyến Ca", CategoryType.WorldTheme),
            CreateCategory(3047, "Linh Khí Khôi Phục", CategoryType.WorldTheme),
            CreateCategory(3048, "Chư Thiên Vạn Giới", CategoryType.WorldTheme),
            CreateCategory(3049, "Nguyên Sinh Huyễn Tưởng", CategoryType.WorldTheme),
            CreateCategory(3050, "Yêu Đương Thường Ngày", CategoryType.WorldTheme),
            CreateCategory(3051, "Diễn Sinh Đồng Nhân", CategoryType.WorldTheme),
            CreateCategory(3052, "Cáo Tiếu Thổ Tào", CategoryType.WorldTheme),

            // Schools / styles
            CreateCategory(4001, "Hệ Thống", CategoryType.Class),
            CreateCategory(4002, "Lão Gia", CategoryType.Class),
            CreateCategory(4003, "Bàn Thờ", CategoryType.Class),
            CreateCategory(4004, "Tùy Thân", CategoryType.Class),
            CreateCategory(4005, "Phàm Nhân", CategoryType.Class),
            CreateCategory(4006, "Vô Địch", CategoryType.Class),
            CreateCategory(4007, "Xuyên Qua", CategoryType.Class),
            CreateCategory(4008, "Nữ Cường", CategoryType.Class),
            CreateCategory(4009, "Khế Ước", CategoryType.Class),
            CreateCategory(4010, "Trọng Sinh", CategoryType.Class),
            CreateCategory(4011, "Hồng Lâu", CategoryType.Class),
            CreateCategory(4012, "Học Viện", CategoryType.Class),
            CreateCategory(4013, "Biến Thân", CategoryType.Class),
            CreateCategory(4014, "Cổ Ngu", CategoryType.Class),
            CreateCategory(4015, "Chuyển Thế", CategoryType.Class),
            CreateCategory(4016, "Xuyên Sách", CategoryType.Class),
            CreateCategory(4017, "Đàn Xuyên", CategoryType.Class),
            CreateCategory(4018, "Phế Tài", CategoryType.Class),
            CreateCategory(4019, "Dưỡng Thành", CategoryType.Class),
            CreateCategory(4020, "Cơm Mềm", CategoryType.Class),
            CreateCategory(4021, "Vô Hạn", CategoryType.Class),
            CreateCategory(4022, "Mary Sue", CategoryType.Class),
            CreateCategory(4023, "Cá Mặn", CategoryType.Class),
            CreateCategory(4024, "Xây Dựng Thế Lực", CategoryType.Class),
            CreateCategory(4025, "Xuyên Nhanh", CategoryType.Class),
            CreateCategory(4026, "Nữ Phụ", CategoryType.Class),
            CreateCategory(4027, "Vả Mặt", CategoryType.Class),
            CreateCategory(4028, "Sảng Văn", CategoryType.Class),
            CreateCategory(4029, "Xuyên Không", CategoryType.Class),
            CreateCategory(4030, "Ngọt Sủng", CategoryType.Class),
            CreateCategory(4031, "Ngự Thú", CategoryType.Class),
            CreateCategory(4032, "Điền Viên", CategoryType.Class),
            CreateCategory(4033, "Toàn Dân", CategoryType.Class),
            CreateCategory(4034, "Mỹ Thực", CategoryType.Class),
            CreateCategory(4035, "Phản Phái", CategoryType.Class),
            CreateCategory(4036, "Sau Màn", CategoryType.Class),
            CreateCategory(4037, "Thiên Tài", CategoryType.Class)
        };

        modelBuilder.Entity<ComicCategory>().HasData(comicCategories);
    }
}
