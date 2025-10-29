using Microsoft.EntityFrameworkCore;
using TruyenCV.Models;

namespace TruyenCV.Models.EntityHardConfig;

public static class UserConfig
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        // Contraint
        modelBuilder.Entity<User>()
            .ToTable(t => t.HasCheckConstraint("CK_User_read_comic_count_Positive", "read_comic_count >= 0"))
            .ToTable(t => t.HasCheckConstraint("CK_User_read_chapter_count_Positive", "read_chapter_count >= 0"))
            .ToTable(t => t.HasCheckConstraint("CK_User_bookmark_count_Positive", "bookmark_count >= 0"))
            .ToTable(t => t.HasCheckConstraint("CK_User_coin_Positive", "coin >= 0"))
            .ToTable(t => t.HasCheckConstraint("CK_User_key_Positive", "key >= 0"));
        // Relationships
        modelBuilder.Entity<User>()
            .HasMany(u => u.RefreshTokens)
            .WithOne(token => token.User)
            .HasForeignKey(token => token.user_id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Permissions)
            .WithOne(permission => permission.User)
            .HasForeignKey(permission => permission.user_id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.PermissionsAssigned)
            .WithOne(permission => permission.AssignedBy)
            .HasForeignKey(permission => permission.assigned_by)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Roles)
            .WithOne(role => role.User)
            .HasForeignKey(role => role.user_id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.RolesAssigned)
            .WithOne(role => role.AssignedBy)
            .HasForeignKey(role => role.assigned_by)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Subscriptions)
            .WithOne(subscription => subscription.User)
            .HasForeignKey(subscription => subscription.user_id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.PaymentHistories)
            .WithOne(history => history.User)
            .HasForeignKey(history => history.user_id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.CoinHistories)
            .WithOne(history => history.User)
            .HasForeignKey(history => history.user_id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.KeyHistories)
            .WithOne(history => history.User)
            .HasForeignKey(history => history.user_id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
