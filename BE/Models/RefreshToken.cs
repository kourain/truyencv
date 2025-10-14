using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TruyenCV.Models
{
    [Table("refresh_tokens")]
    [Index(nameof(token), IsUnique = true)]
    public class RefreshToken : BaseEntity
	{        
        [Required]
        public required string token { get; set; }
        
        [Required]
        [ForeignKey(nameof(User))]
        public required long user_id { get; set; }
        
        [Required]
        public required DateTime expires_at { get; set; }
                
        public DateTime? revoked_at { get; set; }
        [NotMapped]
        public bool is_revoked => revoked_at.HasValue;
        [NotMapped]
        public bool is_expired => DateTime.UtcNow >= expires_at;
        [NotMapped]
        public bool is_active => !is_revoked && !is_expired;
        
        // Navigation property
        [JsonIgnore]
        public User? User { get; set; }
    }
}