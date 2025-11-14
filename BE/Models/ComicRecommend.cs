using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TruyenCV.Models;

[Table("comic_recommends")]
[Index(nameof(comic_id), nameof(month), nameof(year), IsUnique = true)]
public class ComicRecommend : BaseEntity
{
    [Required]
    public required long comic_id { get; set; }

    [Required]
    [Range(0, long.MaxValue, ErrorMessage = "rcm_count không thể nhỏ hơn 0.")]
    public long rcm_count { get; set; } = 0;

    [Range(1, 12)]
    public int month { get; set; }

    [Range(2000, 2100)]
    public int year { get; set; }

    [ForeignKey(nameof(comic_id))]
    [JsonIgnore]
    public virtual Comic? Comic { get; set; } = null;
}
