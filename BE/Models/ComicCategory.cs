using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TruyenCV.Models;

[Table("comic_categories")]
[Index(nameof(name), IsUnique = true)]
public class ComicCategory : BaseEntity
{
	[Required, StringLength(100)]
	public required string name { get; set; }

	[JsonIgnore]
	public virtual ICollection<ComicHaveCategory> ComicHaveCategories { get; set; }
}
