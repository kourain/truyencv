using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TruyenCV.Models;

[Table("comic_comments")]
[Index(nameof(comic_id))]
[Index(nameof(comic_chapter_id))]
[Index(nameof(user_id))]
[Index(nameof(reply_id))]
public class ComicComment : BaseEntity
{
	[Required]
	public required long comic_id { get; set; }

	public long? comic_chapter_id { get; set; }

	[Required]
	public required long user_id { get; set; }

	[Required]
	public required string comment { get; set; }
	public int like { get; set; } = 0;
	public long? reply_id { get; set; }

	public bool is_rate { get; set; } = false;
	
	public int? rate_star { get; set; }

	public bool is_hidden { get; set; } = false;

	[JsonIgnore]
	[ForeignKey(nameof(comic_id))]
	public virtual Comic? Comic { get; set; } = null;

	[JsonIgnore]
	[ForeignKey(nameof(comic_chapter_id))]
	public virtual ComicChapter? ComicChapter { get; set; } = null;

	[JsonIgnore]
	[ForeignKey(nameof(user_id))]
	public virtual User? User { get; set; } = null;

	[JsonIgnore]
	[ForeignKey(nameof(reply_id))]
	public virtual ComicComment? ReplyTo { get; set; } = null;

	[JsonIgnore]
	public virtual ICollection<ComicComment>? Replies { get; set; } = null;
}
