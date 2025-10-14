using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TruyenCV.Models;

[Table("user_comic_bookmarks")]
[Index(nameof(user_id), Name = "IX_UserComicBookmark")]
[Index(nameof(user_id), nameof(comic_id), IsUnique = true)]
public class UserComicBookmark : BaseEntity
{
    public required long user_id { get; set; }
    public required long comic_id { get; set; }

    [ForeignKey(nameof(user_id))]
    [JsonIgnore]
    public virtual User? User { get; set; }

    [ForeignKey(nameof(comic_id))]
    [JsonIgnore]
    public virtual Comic? Comic { get; set; }
}