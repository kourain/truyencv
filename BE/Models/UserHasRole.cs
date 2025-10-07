
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
namespace TruyenCV.Models;

[Table("user_has_roles")]
public class UserHasRole : BaseEntity
{
	[Required]
	public required string role_name { get; set; }
    [Required]
    public required long user_id {get;set;}
    [Required]
    public required long assigned_by {get;set;}
    [ForeignKey(nameof(user_id))]
    [JsonIgnore]
    public virtual User? User {get;set;}
    [ForeignKey(nameof(assigned_by))]
    [JsonIgnore]
    public virtual User? AssignedBy {get;set;}
}
