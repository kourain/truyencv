using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TruyenCV.Models;

[Table("comics")]
[Index(nameof(slug), IsUnique = true)]
public class Comic : BaseEntity
{
	[Required, StringLength(255)]
	public required string name { get; set; }

	[Required]
	public required string description { get; set; }

	[Required, StringLength(255)]
	public required string slug { get; set; }

	[Required, StringLength(255)]
	public required string author { get; set; }

	[StringLength(255)]
	public string? embedded_from { get; set; }

	[StringLength(500)]
	public string? embedded_from_url { get; set; }

	public int chap_count { get; set; } = 0;

	public float rate { get; set; } = 0;

	[Required]
	public ComicStatus status { get; set; } = ComicStatus.Continuing;

	[JsonIgnore]
	public ICollection<ComicChapter>? ComicChapters { get; set; }

	[JsonIgnore]
	public ICollection<ComicComment>? ComicComments { get; set; }

	[JsonIgnore]
	public ICollection<ComicHaveCategory>? ComicHaveCategories { get; set; }
}
