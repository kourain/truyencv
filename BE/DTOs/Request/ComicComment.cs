namespace TruyenCV.DTO.Request;

// Request DTOs for creating and updating comic comments
public class CreateComicCommentRequest
{
	public required ulong comic_id { get; set; }
	public ulong? comic_chapter_id { get; set; }
	public required ulong user_id { get; set; }
	public required string comment { get; set; }
	public int like { get; set; } = 0;
	public ulong? reply_id { get; set; }
	public bool is_rate { get; set; } = false;
	public int? rate_star { get; set; }
}

public class UpdateComicCommentRequest
{
	public required ulong id { get; set; }
	public required ulong comic_id { get; set; }
	public ulong? comic_chapter_id { get; set; }
	public required ulong user_id { get; set; }
	public required string comment { get; set; }
	public int like { get; set; }
	public ulong? reply_id { get; set; }
	public bool is_rate { get; set; }
	public int? rate_star { get; set; }
}
