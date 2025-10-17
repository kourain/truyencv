using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TruyenCV.Models;

[Table("comic_reports")]
[Index(nameof(comic_id), nameof(chapter_id), nameof(comment_id))]
public class ComicReport : BaseEntity
{
    [Required]
    public required long comic_id { get; set; }

    public long? chapter_id { get; set; }

    public long? comment_id { get; set; }

    [Required]
    public required long reporter_id { get; set; }

    [Required]
    [StringLength(500)]
    public required string reason { get; set; }

    [Required]
    public ReportStatus status { get; set; } = ReportStatus.Pending;

    [ForeignKey(nameof(comic_id))]
    [JsonIgnore]
    public virtual Comic? Comic { get; set; }

    [ForeignKey(nameof(chapter_id))]
    [JsonIgnore]
    public virtual ComicChapter? Chapter { get; set; }

    [ForeignKey(nameof(comment_id))]
    [JsonIgnore]
    public virtual ComicComment? Comment { get; set; }

    [ForeignKey(nameof(reporter_id))]
    [JsonIgnore]
    public virtual User? Reporter { get; set; }
}
