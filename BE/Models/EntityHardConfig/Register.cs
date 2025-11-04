using System;
using Microsoft.EntityFrameworkCore;
using TruyenCV.Models;

namespace TruyenCV.Models.EntityHardConfig;

public static class Register
{
    public static void EntityHardConfig(this ModelBuilder modelBuilder)
    {
        ComicCommentConfig.Configure(modelBuilder);
        ComicConfig.Configure(modelBuilder);
        ComicHaveCategoryConfig.Configure(modelBuilder);
        SubscriptionConfig.Configure(modelBuilder);
        UserConfig.Configure(modelBuilder);
    }
}