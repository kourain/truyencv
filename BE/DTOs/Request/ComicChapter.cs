namespace TruyenCV.DTO.Request;

// Request DTOs for creating and updating comic chapters
public class CreateComicChapterRequest
{
	public required long comic_id { get; set; }
	public required int chapter { get; set; }
	public required string content { get; set; }
}

public class UpdateComicChapterRequest
{
	public required long id { get; set; }
	public required long comic_id { get; set; }
	public required int chapter { get; set; }
	public required string content { get; set; }
}
