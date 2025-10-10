using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TruyenCV.Models;

[Table("user_comic_read_history")]
[Index(nameof(user_id), Name = "IX_UserComicReadHistory")]
[Index(nameof(user_id), nameof(comic_id), IsUnique = true)]
public class UserComicReadHistory : BaseEntity
{
    public required ulong user_id { get; set; }
    public required ulong comic_id { get; set; }
    public required ulong chapter_id { get; set; }

    [NotMapped]
    public DateTime read_at => updated_at;

    [ForeignKey(nameof(user_id))]
    [JsonIgnore]
    public virtual User? User { get; set; }

    [ForeignKey(nameof(comic_id))]
    [JsonIgnore]
    public virtual Comic? Comic { get; set; }
}