
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TruyenCV.Helpers;
using Microsoft.EntityFrameworkCore;

namespace TruyenCV.Models;

public class DataContext : Microsoft.EntityFrameworkCore.DbContext
{
	public DataContext(DbContextOptions<DataContext> options) : base(options) { }
	public DbSet<User> Users { get; set; }
	public DbSet<RefreshToken> RefreshTokens { get; set; }
	public DbSet<ComicCategory> ComicCategories { get; set; }
	public DbSet<Comic> Comics { get; set; }
	public DbSet<ComicChapter> ComicChapters { get; set; }
	public DbSet<ComicComment> ComicComments { get; set; }
	public DbSet<ComicHaveCategory> ComicHaveCategories { get; set; }
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<User>().HasData(
			new User
			{
				name = "kourain",
				email = "maiquyen16503@gmail.com",
				password = Bcrypt.HashPassword("1408"), // Nên mã hóa mật khẩu trong thực tế!
				phone = "0123456789",
				created_at = DateTime.UtcNow,
				updated_at = DateTime.UtcNow
			}
		);
	}
	public override int SaveChanges()
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
					entry.State = EntityState.Modified;
					entry.Entity.deleted_at = DateTime.UtcNow;
					break;
				default:
					break;
			}
		}
		return base.SaveChanges();
	}
	public override async System.Threading.Tasks.Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken = default)
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
					entry.State = EntityState.Modified;
					entry.Entity.deleted_at = DateTime.UtcNow;
					break;
				default:
					break;
			}
		}
		return await base.SaveChangesAsync(cancellationToken);
	}
}
[PrimaryKey(nameof(id)), Index(nameof(id), IsUnique = true)]
public abstract class BaseEntity
{
	public long id { get; set; } = SnowflakeIdGenerator.NextId();
	public DateTime? deleted_at { get; set; }
	public DateTime created_at { get; set; }
	public DateTime updated_at { get; set; }
}