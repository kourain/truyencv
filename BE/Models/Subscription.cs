using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TruyenCV.Models;

[Table("subscriptions")]
[Index(nameof(code), IsUnique = true)]
public class Subscription : BaseEntity
{
    [Required, StringLength(50)]
    public required string code { get; set; }

    [Required, StringLength(100)]
    public required string name { get; set; }

    [StringLength(255)]
    public string? description { get; set; }

    public long price_coin { get; set; } = 0;

    public int duration_day { get; set; } = 0;

    public bool is_active { get; set; } = true;

    [JsonIgnore, DeleteBehavior(DeleteBehavior.Cascade)]
    public virtual ICollection<UserHasSubscription>? UserSubscriptions { get; set; }
}
