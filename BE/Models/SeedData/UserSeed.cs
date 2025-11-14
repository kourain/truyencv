using Microsoft.EntityFrameworkCore;
using TruyenCV.Helpers;

namespace TruyenCV.Models.SeedData;
public static class UserSeed
{
    public static void Seed(ModelBuilder modelBuilder, DateTime defaultDate)
    {
        var system = new User()
        {
            id = SystemUser.id,
            name = "System",
            email = "ht.kourain@gmail.com",
            password = "$2a$12$2TFakadQVfOIOz1XsDPhkOFBHFFKSzJMtjyUkkIBJokTaRgiY6LJa",
            phone = "0000000000",
            email_verified_at = defaultDate,
            read_comic_count = 0,
            read_chapter_count = 0,
            bookmark_count = 0,
            coin = 0,
            key = 0,
            is_banned = false,
            created_at = defaultDate,
            updated_at = defaultDate
        };
        var baseUser = new User()
        {
            id = 766206485104431104L,
            name = "kourain",
            email = "maiquyen16503@gmail.com",
            password = "$2a$12$DF.qp7O4zo5G.OIaYOxXouJqIIiUMJ5r/67yqJHi7cjtr7WAkbZnu", // BCrypt cho mật khẩu mẫu
            phone = "0123456789",
            email_verified_at = defaultDate,
            read_comic_count = 0,
            read_chapter_count = 0,
            bookmark_count = 0,
            coin = 0,
            key = 0,
            is_banned = false,
            created_at = defaultDate,
            updated_at = defaultDate
        };
        modelBuilder.Entity<User>().HasData([system, baseUser]);
        modelBuilder.Entity<UserHasRole>().HasData(
        [
            new UserHasRole
            {
                id = 766206486589214720L,
                user_id = baseUser.id,
                role_name = Roles.Admin,
                assigned_by = system.id,
                created_at = defaultDate,
                updated_at = defaultDate
            },
            new UserHasRole
            {
                id = 766206486593409021L,
                user_id = baseUser.id,
                role_name = Roles.Converter,
                assigned_by = system.id,
                created_at = defaultDate,
                updated_at = defaultDate
            },
            new UserHasRole
            {
                id = 766206486593409024L,
                user_id = baseUser.id,
                role_name = Roles.User,
                assigned_by = system.id,
                created_at = defaultDate,
                updated_at = defaultDate
            },
            new UserHasRole
            {
                id = 766206486593409025L,
                user_id = system.id,
                role_name = Roles.System,
                assigned_by = system.id,
                created_at = defaultDate,
                updated_at = defaultDate
            }
        ]
        );
    }
}