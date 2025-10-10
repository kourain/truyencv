using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TruyenCV.Models;

[Table("user_has_permissions")]
[Index(nameof(user_id), Name = "IX_UserHasPermission")]
[Index(nameof(user_id), nameof(permissions), IsUnique = true)]
public class UserHasPermission : BaseEntity
{
	[Required]
	public required Permissions permissions { get; set; }

    [Required]
    public required ulong user_id { get; set; }

    [Required]
    public required ulong assigned_by { get; set; }

    [ForeignKey(nameof(user_id))]
    [JsonIgnore]
    public virtual User? User { get; set; }

    [ForeignKey(nameof(assigned_by))]
    [JsonIgnore]
    public virtual User? AssignedBy { get; set; }
}
