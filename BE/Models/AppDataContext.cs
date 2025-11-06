
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TruyenCV.Helpers;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;
using TruyenCV.Models.EntityHardConfig;
using TruyenCV.Models.SeedData;

namespace TruyenCV.Models;

public class AppDataContext : Microsoft.EntityFrameworkCore.DbContext
{
    public AppDataContext(DbContextOptions<AppDataContext> options) : base(options) { }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<ComicCategory> ComicCategories { get; set; }
    public DbSet<Comic> Comics { get; set; }
    public DbSet<ComicChapter> ComicChapters { get; set; }
    public DbSet<ComicComment> ComicComments { get; set; }
    public DbSet<ComicHaveCategory> ComicHaveCategories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserHasRole> UserHasRoles { get; set; }
    public DbSet<UserComicBookmark> UserComicBookmarks { get; set; }
    public DbSet<UserComicReadHistory> UserComicReadHistories { get; set; }
    public DbSet<UserHasPermission> UserHasPermissions { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<UserHasSubscription> UserHasSubscriptions { get; set; }
    public DbSet<PaymentHistory> PaymentHistories { get; set; }
    public DbSet<UserUseCoinHistory> UserCoinHistories { get; set; }
    public DbSet<UserUseKeyHistory> UserUseKeyHistories { get; set; }
    public DbSet<UserComicUnlockHistory> UserComicUnlockHistories { get; set; }
    public DbSet<UserComicRecommend> UserComicRecommends { get; set; }
    public DbSet<ComicRecommend> ComicRecommends { get; set; }
    public DbSet<ComicReport> ComicReports { get; set; }
    public static readonly DateTime defaultDate = DateTime.Parse("2025-09-23T00:00:00Z").ToUniversalTime();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.EntityHardConfig();
        modelBuilder.SeedData(defaultDate);
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var navigation in entityType.GetNavigations())
            {
                navigation.SetIsEagerLoaded(false);
            }
        }
        base.OnModelCreating(modelBuilder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
    private void Save()
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.created_at = DateTime.UtcNow;
                    entry.Entity.updated_at = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.updated_at = DateTime.UtcNow;
                    break;
                case EntityState.Deleted:
                    if (isSoftDelete)
                    {
                        entry.State = EntityState.Modified;
                        entry.Entity.deleted_at = DateTime.UtcNow;
                    }
                    break;
                default:
                    break;
            }
        }
        isSoftDelete = true;
    }
    public bool isSoftDelete { get; set; } = true;
    public override int SaveChanges()
    {
        Save();
        return base.SaveChanges();
    }
    public AppDataContext NotSoftDelete()
    {
        isSoftDelete = false;
        return this;
    }
    public override async System.Threading.Tasks.Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken = default)
    {
        Save();
        return await base.SaveChangesAsync(cancellationToken);
    }
}
[PrimaryKey(nameof(id))]
public abstract class BaseEntity
{
    [Key]
    public long id { get; set; } = SnowflakeIdGenerator.NextId();
    [NotMapped]
    public string _id { get => id.ToString(); set => id = long.Parse(value); }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public DateTime? deleted_at { get; set; }
}