using Microsoft.EntityFrameworkCore;
using TruyenCV.Models;

namespace TruyenCV.Models.ConfigHardFKey;

public static class SubscriptionConfig
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Subscription>()
            .HasMany(subscription => subscription.UserSubscriptions)
            .WithOne(link => link.Subscription)
            .HasForeignKey(link => link.subscription_id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
