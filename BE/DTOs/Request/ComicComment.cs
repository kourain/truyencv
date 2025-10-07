namespace TruyenCV.DTO.Request;

// Request DTOs for creating and updating comic comments
public class CreateComicCommentRequest
{
	public required long comic_id { get; set; }
	public long? comic_chapter_id { get; set; }
	public required long user_id { get; set; }
	public required string comment { get; set; }
	public int like { get; set; } = 0;
	public long? reply_id { get; set; }
	public bool is_rate { get; set; } = false;
	public int? rate_star { get; set; }
}

public class UpdateComicCommentRequest
{
	public required long id { get; set; }
	public required long comic_id { get; set; }
	public long? comic_chapter_id { get; set; }
	public required long user_id { get; set; }
	public required string comment { get; set; }
	public int like { get; set; }
	public long? reply_id { get; set; }
	public bool is_rate { get; set; }
	public int? rate_star { get; set; }
}
