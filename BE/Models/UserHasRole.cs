
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
namespace TruyenCV.Models;

[Table("user_has_roles")]
[Index(nameof(user_id), Name = "IX_UserHasRole")]
public class UserHasRole : BaseEntity
{
	[Required]
	public required string role_name { get; set; }
    [Required]
    public required long user_id {get;set;}
    [Required]
    public required long assigned_by {get;set;}
    [JsonIgnore]
    public virtual User? User {get;set;}
    [JsonIgnore]
    public virtual User? AssignedBy {get;set;}

    public DateTime? revoked_at { get; set; }

    [NotMapped]
    public bool is_active => revoked_at == null && deleted_at == null;
}
