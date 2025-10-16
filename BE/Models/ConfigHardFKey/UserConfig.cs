using Microsoft.EntityFrameworkCore;
using TruyenCV.Models;

namespace TruyenCV.Models.ConfigHardFKey;

public static class UserConfig
{
    public static void Configure(ModelBuilder modelBuilder)
    {
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
