using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TruyenCV.Models;

[Table("user_has_permissions")]
[Index(nameof(user_id), Name = "IX_UserHasPermission")]
public class UserHasPermission : BaseEntity
{
	[Required]
	public required Permissions permissions { get; set; }

    [Required]
    public required long user_id { get; set; }

    [Required]
    public required long assigned_by { get; set; }

    [JsonIgnore]
    public virtual User? User { get; set; }

    [JsonIgnore]
    public virtual User? AssignedBy { get; set; }
}
