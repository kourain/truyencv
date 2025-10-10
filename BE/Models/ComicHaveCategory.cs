using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TruyenCV.Models;

[Table("comic_have_categories")]
[PrimaryKey(nameof(comic_id), nameof(comic_category_id))]
[Index(nameof(comic_id))]
[Index(nameof(comic_category_id))]
public class ComicHaveCategory
{
	[Required]
	public required ulong comic_id { get; set; }

	[Required]
	public required ulong comic_category_id { get; set; }

	[JsonIgnore]
	[ForeignKey(nameof(comic_id))]
	public Comic? Comic { get; set; }

	[JsonIgnore]
	[ForeignKey(nameof(comic_category_id))]
	public ComicCategory? ComicCategory { get; set; }
}
