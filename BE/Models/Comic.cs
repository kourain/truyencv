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
    [ForeignKey(nameof(EmbeddedByUser))]
    public long embedded_by { get; set; }
    public int chapter_count { get; set; } = 0;
    public int bookmark_count { get; set; } = 0;
    public int? published_year { get; set; } = (int)DateTime.UtcNow.Year;

    public float rate { get; set; } = 0;

    [Required]
    public ComicStatus status { get; set; } = ComicStatus.Continuing;

    public virtual User? EmbeddedByUser { get; set; }
    [JsonIgnore] //, DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual ICollection<ComicChapter>? ComicChapters { get; set; }

    [JsonIgnore] //, DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual ICollection<ComicComment>? ComicComments { get; set; }

    [JsonIgnore] //, DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual ICollection<ComicHaveCategory>? ComicHaveCategories { get; set; }
}
