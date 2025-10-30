using Microsoft.EntityFrameworkCore;
using TruyenCV.Models;

namespace TruyenCV.Models.EntityHardConfig;

public static class ComicConfig
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comic>()
            .ToTable(t=> t.HasCheckConstraint("CK_Comic_bookmark_count_Positive", "bookmark_count >= 0"))
            .ToTable(t=> t.HasCheckConstraint("CK_Comic_chapter_count_Positive", "chapter_count >= 0"));
        modelBuilder.HasPostgresExtension("vector");

        modelBuilder.Entity<Comic>()
            .Property(c => c.search_vector)
            .HasColumnType($"vector({EmbeddingDefaults.Dimensions})");
    }
}
