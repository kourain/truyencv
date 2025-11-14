using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TruyenCV.Models;

[Table("user_has_subscriptions")]
[Index(nameof(user_id), nameof(subscription_id), IsUnique = true)]
public class UserHasSubscription : BaseEntity
{
    [Required]
    public required long user_id { get; set; }

    [Required]
    public required long subscription_id { get; set; }

    public DateTime start_at { get; set; } = DateTime.UtcNow;

    public DateTime? end_at { get; set; }

    public bool is_active { get; set; } = true;

    public bool auto_renew { get; set; } = false;

    [ForeignKey(nameof(user_id))]
    [JsonIgnore]
    public virtual User? User { get; set; } = null;

    [ForeignKey(nameof(subscription_id))]
    [JsonIgnore]
    public virtual Subscription? Subscription { get; set; } = null;
}
