using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TruyenCV.Models;

[Table("user_comic_recommends")]
[Index(nameof(user_id), nameof(month), nameof(year), IsUnique = true)]
[Index(nameof(comic_id))]
public class UserComicRecommend : BaseEntity
{
    [Required]
    public required long user_id { get; set; }

    [Required]
    public required long comic_id { get; set; }

    [Range(1, 12)]
    public int month { get; set; }

    [Range(2000, 2100)]
    public int year { get; set; }

    [ForeignKey(nameof(user_id))]
    [JsonIgnore]
    public virtual User? User { get; set; }

    [ForeignKey(nameof(comic_id))]
    [JsonIgnore]
    public virtual Comic? Comic { get; set; }
}
