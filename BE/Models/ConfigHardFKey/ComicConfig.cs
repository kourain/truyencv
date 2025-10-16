using Microsoft.EntityFrameworkCore;
using TruyenCV.Models;

namespace TruyenCV.Models.ConfigHardFKey;

public static class ComicConfig
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");

        modelBuilder.Entity<Comic>()
            .Property(c => c.search_vector)
            .HasColumnType($"vector({EmbeddingDefaults.Dimensions})");
    }
}
