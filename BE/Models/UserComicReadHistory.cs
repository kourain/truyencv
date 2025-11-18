using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TruyenCV.DTOs.Response;

namespace TruyenCV.Models;

[Table("user_comic_read_history")]
[Index(nameof(user_id), Name = "IX_UserComicReadHistory")]
[Index(nameof(user_id), nameof(comic_id), IsUnique = true)]
public class UserComicReadHistory : BaseEntity
{
    [ForeignKey(nameof(User))]
    public required long user_id { get; set; }
    [ForeignKey(nameof(Comic))]
    public required long comic_id { get; set; }
    [ForeignKey(nameof(ComicChapter))]
    public required long chapter_id { get; set; }
    [NotMapped]
    public DateTime read_at => updated_at;

    [JsonIgnore]
    [ForeignKey(nameof(user_id))]
    public virtual User? User { get; set; } = null;

    [ForeignKey(nameof(comic_id))]
    public virtual Comic? Comic { get; set; }
    [ForeignKey(nameof(chapter_id))]
    public virtual ComicChapter? ComicChapter { get; set; } = null;
}