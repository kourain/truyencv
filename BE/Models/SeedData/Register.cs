using System;
using Microsoft.EntityFrameworkCore;
using TruyenCV.Models;

namespace TruyenCV.Models.SeedData;

public static class Register
{
    public static void SeedData(this ModelBuilder modelBuilder, DateTime defaultDate)
    {
        ComicCategorySeed.Seed(modelBuilder, defaultDate);
        UserHasRoleSeed.Seed(modelBuilder, defaultDate);
        UserSeed.Seed(modelBuilder, defaultDate);
    }
}