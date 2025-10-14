using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TruyenCV.Const;

namespace TruyenCV.Models;

[Table("user_use_key_history")]
[Index(nameof(user_id), Name = "IX_UserUseKeyHistory_User")]
public class UserUseKeyHistory : BaseEntity
{
    [Required]
    public required long user_id { get; set; }

    public long key { get; set; } = 0;

    [Required]
    public required HistoryStatus status { get; set; }

    public long? chapter_id { get; set; }

    [StringLength(255)]
    public string? note { get; set; }

    [ForeignKey(nameof(user_id))]
    [JsonIgnore]
    public virtual User? User { get; set; }

    [ForeignKey(nameof(chapter_id))]
    [JsonIgnore]
    public virtual ComicChapter? Chapter { get; set; }
}
