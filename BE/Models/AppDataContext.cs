
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TruyenCV.Helpers;
using Microsoft.EntityFrameworkCore;

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
    public static readonly DateTime defaultDate = DateTime.Parse("2025-09-23T00:00:00Z").ToUniversalTime();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
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
        var system = new User()
        {
            id = SystemUser.id,
            name = "System",
            email = "ht.kourain@gmail.com",
            password = Bcrypt.HashPassword("159753"),
            phone = "0000000000",
            email_verified_at = defaultDate,
            read_comic_count = 0,
            read_chapter_count = 0,
            bookmark_count = 0,
            coin = 0,
            is_banned = false,
            created_at = defaultDate,
            updated_at = defaultDate
        };
        var baseUser = new User()
        {
            name = "kourain",
            email = "maiquyen16503@gmail.com",
            password = Bcrypt.HashPassword("1408"), // Nên mã hóa mật khẩu trong thực tế!
            phone = "0123456789",
            email_verified_at = defaultDate,
            read_comic_count = 0,
            read_chapter_count = 0,
            bookmark_count = 0,
            coin = 0,
            is_banned = false,
            created_at = defaultDate,
            updated_at = defaultDate
        };
        modelBuilder.Entity<User>().HasData([system, baseUser]);
        modelBuilder.Entity<UserHasRole>().HasData(
        [
            new UserHasRole
            {
                user_id = baseUser.id,
                role_name = Roles.Admin,
                assigned_by = system.id,
                created_at = defaultDate,
                updated_at = defaultDate
            },
            new UserHasRole
            {
                user_id = baseUser.id,
                role_name = Roles.User,
                assigned_by = system.id,
                created_at = defaultDate,
                updated_at = defaultDate
            },
            new UserHasRole
            {
                user_id = system.id,
                role_name = Roles.System,
                assigned_by = system.id,
                created_at = defaultDate,
                updated_at = defaultDate
            }
        ]
        );
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