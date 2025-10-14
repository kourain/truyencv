using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TruyenCV.Models;

[Table("comic_chapters")]
[Index(nameof(comic_id), nameof(chapter), IsUnique = true)]
public class ComicChapter : BaseEntity
{
	[Required]
	public required long comic_id { get; set; }

	[Required]
	public required int chapter { get; set; }

	[Required]
	public required string content { get; set; }

	/// <summary>
	/// Số lượng chìa khóa cần để mở khóa chương. Mặc định là 1.
	/// </summary>
	public int key_require { get; set; } = 1;

	/// <summary>
	/// Nếu được thiết lập, chương chỉ yêu cầu chìa khóa đến thời điểm này.
	/// </summary>
	public DateTime? key_require_until { get; set; }

	[JsonIgnore]
	[ForeignKey(nameof(comic_id))]
	public virtual Comic? Comic { get; set; }

	[JsonIgnore, DeleteBehavior(DeleteBehavior.SetNull)]
	public virtual ICollection<ComicComment>? ComicComments { get; set; }
}
