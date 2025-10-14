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
	[ForeignKey(nameof(Comic))]
	public required long comic_id { get; set; }

	[Required]
	[ForeignKey(nameof(ComicCategory))]
	public required long comic_category_id { get; set; }

	[JsonIgnore]
	public virtual Comic? Comic { get; set; }

	[JsonIgnore]
	public virtual ComicCategory? ComicCategory { get; set; }
}
