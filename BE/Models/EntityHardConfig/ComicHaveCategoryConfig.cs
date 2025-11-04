using Microsoft.EntityFrameworkCore;
using TruyenCV.Models;

namespace TruyenCV.Models.EntityHardConfig;

public static class ComicHaveCategoryConfig
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ComicHaveCategory>()
            // .ToTable(t=> t.HasCheckConstraint("CK_comic_have_categories_comic_id", "comic_id > 0"))
            .ToTable(t=> t.HasCheckConstraint("CK_comic_have_categories_category_id", "category_id > 2000"));
    }
}
