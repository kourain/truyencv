using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TruyenCV.Const;

namespace TruyenCV.Models;

[Table("user_coin_history")]
[Index(nameof(user_id), Name = "IX_UserCoinHistory_User")]
public class UserCoinHistory : BaseEntity
{
    [Required]
    public required long user_id { get; set; }

    public long coin { get; set; } = 0;

    [Required]
    public required HistoryStatus status { get; set; }

    [StringLength(255)]
    public string? reason { get; set; }

    public long? reference_id { get; set; }

    [StringLength(100)]
    public string? reference_type { get; set; }

    [ForeignKey(nameof(user_id))]
    [JsonIgnore]
    public virtual User? User { get; set; }
}
