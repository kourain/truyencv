namespace TruyenCV.DTOs.Request;

// Request DTOs for creating and updating comic comments
public class CreateComicCommentRequest
{
	public required string comic_id { get; set; }
	public string? comic_chapter_id { get; set; }
	public required string user_id { get; set; }
	public required string comment { get; set; }
	public int like { get; set; } = 0;
	public string? reply_id { get; set; }
	public bool is_rate { get; set; } = false;
	public int? rate_star { get; set; }
}

public class UpdateComicCommentRequest
{
	public required string id { get; set; }
	public required string comic_id { get; set; }
	public string? comic_chapter_id { get; set; }
	public required string user_id { get; set; }
	public required string comment { get; set; }
	public int like { get; set; }
	public string? reply_id { get; set; }
	public bool is_rate { get; set; }
	public int? rate_star { get; set; }
}
