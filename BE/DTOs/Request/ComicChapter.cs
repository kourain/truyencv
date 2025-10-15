namespace TruyenCV.DTOs.Request;

// Request DTOs for creating and updating comic chapters
public class CreateComicChapterRequest
{
	public required string comic_id { get; set; }
	public required int chapter { get; set; }
	public required string content { get; set; }
}

public class UpdateComicChapterRequest
{
	public required string id { get; set; }
	public required string comic_id { get; set; }
	public required int chapter { get; set; }
	public required string content { get; set; }
}
