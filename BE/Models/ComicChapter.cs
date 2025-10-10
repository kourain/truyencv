using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TruyenCV.Models;

[Table("comic_chapters")]
[Index(nameof(comic_id), nameof(chapter), IsUnique = true)]
public class ComicChapter : BaseEntity
{
	[Required]
	public required ulong comic_id { get; set; }

	[Required]
	public required int chapter { get; set; }

	[Required]
	public required string content { get; set; }

	[JsonIgnore]
	[ForeignKey(nameof(comic_id))]
	public Comic? Comic { get; set; }

	[JsonIgnore]
	public ICollection<ComicComment>? ComicComments { get; set; }
}
