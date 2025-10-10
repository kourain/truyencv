using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
namespace TruyenCV.Models;

[Table("users")]
[Index(nameof(email), IsUnique = true)]
public class User : BaseEntity
{
    [Required]
    public required string name { get; set; }
    [Required, StringLength(256)]
    public required string email { get; set; }
    [Required, StringLength(60)]
    public string password { get; set; }
    [Required, StringLength(15)]
    public string phone { get; set; }
    public ulong comic_read_count { get; set; }
    [Required, StringLength(50)]
    public string avatar { get; set; } = "default_avatar.png";
    public ICollection<RefreshToken>? RefreshTokens { get; set; }
    public ICollection<UserHasPermission>? Permissions { get; set; }
    public ICollection<UserHasRole>? Roles { get; set; }
}
