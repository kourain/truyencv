using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TruyenCV.Models;

[Table("payment_history")]
[Index(nameof(user_id), Name = "IX_PaymentHistory_User")]
public class PaymentHistory : BaseEntity
{
    [Required]
    public required long user_id { get; set; }

    public long amount_coin { get; set; } = 0;

    public long amount_money { get; set; } = 0;

    [StringLength(50)]
    public string payment_method { get; set; } = "unknown";

    [StringLength(100)]
    public string? reference_id { get; set; }

    [StringLength(255)]
    public string? note { get; set; }

    [ForeignKey(nameof(user_id))]
    [JsonIgnore]
    public virtual User? User { get; set; }
}
