
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TruyenCV.Models;

[Table("Roles")]
public class Role : BaseEntity
{
	[Required]
	public required string name { get; set; }
	[Required]
	public required string title { get; set; }
}
