using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TruyenCV.Models;

[Table("user_comic_unlock_history")]
[Index(nameof(user_id), Name = "IX_UserComicUnlockHistory")]
[Index(nameof(user_id), nameof(comic_id), nameof(comic_chapter_id), IsUnique = true)]
public class UserComicUnlockHistory : BaseEntity
{
	public required long user_id { get; set; }

	public required long comic_id { get; set; }

	public required long comic_chapter_id { get; set; }

	[ForeignKey(nameof(user_id))]
	[JsonIgnore]
	public virtual User? User { get; set; } = null;

	[ForeignKey(nameof(comic_id))]
	[JsonIgnore]
	public virtual Comic? Comic { get; set; } = null;

	[ForeignKey(nameof(comic_chapter_id))]
	[JsonIgnore]
	public virtual ComicChapter? ComicChapter { get; set; } = null;
}
