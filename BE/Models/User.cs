using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
namespace TruyenCV.Models;

[Table("users")]
[Index(nameof(email), IsUnique = true)]
[Index(nameof(firebase_uid), IsUnique = true)]
public class User : BaseEntity
{
    [Required]
    public required string name { get; set; }
    public string? firebase_uid { get; set; } = null;
    [Required, StringLength(256)]
    public required string email { get; set; }
    [Required, StringLength(60)]
    public string password { get; set; }
    [Required, StringLength(15)]
    public string phone { get; set; }
    public DateTime? email_verified_at { get; set; }
    [Range(0, long.MaxValue, ErrorMessage = "read_comic_count không thể nhỏ hơn 0.")]
    public long read_comic_count { get; set; } = 0;
    [Range(0, long.MaxValue, ErrorMessage = "read_chapter_count không thể nhỏ hơn 0.")]
    public long read_chapter_count { get; set; } = 0;
    [Range(0, long.MaxValue, ErrorMessage = "bookmark_count không thể nhỏ hơn 0.")]
    public long bookmark_count { get; set; } = 0;
    [Range(0, long.MaxValue, ErrorMessage = "coin không thể nhỏ hơn 0.")]
    public long coin { get; set; } = 0;
    [Range(0, long.MaxValue, ErrorMessage = "key không thể nhỏ hơn 0.")]
    public long key { get; set; } = 0;
    public bool is_banned { get; set; } = false;
    public DateTime? banned_at { get; set; }
    [Required, StringLength(15360)] // 15 KB for avatar image in base64
    public string avatar { get; set; } = "default_avatar.png";
    [JsonIgnore, DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual ICollection<RefreshToken>? RefreshTokens { get; set; } = null;

    [JsonIgnore, DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual ICollection<UserHasPermission>? Permissions { get; set; } = null;

    [JsonIgnore, DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual ICollection<UserHasPermission>? PermissionsAssigned { get; set; } = null;

    [JsonIgnore, DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual ICollection<UserHasRole>? Roles { get; set; } = null;
    [JsonIgnore, DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual ICollection<UserHasRole>? RolesAssigned { get; set; } = null;

    [JsonIgnore, DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual ICollection<UserHasSubscription>? Subscriptions { get; set; } = null;

    [JsonIgnore, DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual ICollection<PaymentHistory>? PaymentHistories { get; set; } = null;

    [JsonIgnore, DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual ICollection<UserUseCoinHistory>? CoinHistories { get; set; } = null;

    [JsonIgnore, DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual ICollection<UserUseKeyHistory>? KeyHistories { get; set; } = null;

}
