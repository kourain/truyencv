using Microsoft.EntityFrameworkCore;
using TruyenCV.Models;

namespace TruyenCV.Models.EntityHardConfig;

public static class ComicConfig
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        // Ensure postgres vector extension is declared
        modelBuilder.HasPostgresExtension("vector");

        // Check constraints for non-negative counts (use ToTable/HasCheckConstraint as recommended)
        modelBuilder.Entity<Comic>()
            .ToTable(t => t.HasCheckConstraint("CK_Comic_bookmark_count_Positive", "bookmark_count >= 0"))
            .ToTable(t => t.HasCheckConstraint("CK_Comic_chapter_count_Positive", "chapter_count >= 0"))
            .ToTable(t => t.HasCheckConstraint("CK_Comic_main_category_id_Valid", "main_category_id < 2000"));

        // Configure search vector column
        modelBuilder.Entity<Comic>()
            .Property(c => c.search_vector)
            .HasColumnType($"vector({EmbeddingDefaults.Dimensions})");

        // Relationships
        // Comic -> ComicChapters (1 - many)
        modelBuilder.Entity<Comic>()
            .HasMany(c => c.ComicChapters)
            .WithOne(ch => ch.Comic)
            .HasForeignKey(ch => ch.comic_id)
            .OnDelete(DeleteBehavior.Cascade);

        // Comic -> ComicComments (1 - many)
        modelBuilder.Entity<Comic>()
            .HasMany(c => c.ComicComments)
            .WithOne(cm => cm.Comic)
            .HasForeignKey(cm => cm.comic_id)
            .OnDelete(DeleteBehavior.Cascade);

        // Comic -> EmbeddedByUser (many comics can be embedded by one user)
        modelBuilder.Entity<Comic>()
            .HasOne(c => c.EmbeddedByUser)
            .WithMany()
            .HasForeignKey(c => c.embedded_by)
            .OnDelete(DeleteBehavior.Restrict);

        // Comic -> AcceptByUser
        modelBuilder.Entity<Comic>()
            .HasOne(c => c.AcceptByUser)
            .WithMany()
            .HasForeignKey(c => c.accept_by)
            .OnDelete(DeleteBehavior.Restrict);

        // Comic -> MainCategory (FK to comic_categories)
        modelBuilder.Entity<Comic>()
            .HasOne(c => c.MainCategory)
            .WithMany()
            .HasForeignKey(c => c.main_category_id)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
