using Microsoft.EntityFrameworkCore;
using TruyenCV.Models;

namespace TruyenCV.Models.EntityHardConfig;

public static class ComicCommentConfig
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ComicComment>()
            .ToTable(t=> t.HasCheckConstraint("CK_comic_comments_rate_star_range", "(rate_star IS NULL) OR (rate_star BETWEEN 1 AND 5)"));
    }
}
