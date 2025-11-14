using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TruyenCV.Models;

[Table("user_use_coin_history")]
[Index(nameof(user_id), Name = "IX_UserUseCoinHistory_User")]
public class UserUseCoinHistory : BaseEntity
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
    public virtual User? User { get; set; } = null;
}
