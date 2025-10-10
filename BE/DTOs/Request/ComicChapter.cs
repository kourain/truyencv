namespace TruyenCV.DTO.Request;

// Request DTOs for creating and updating comic chapters
public class CreateComicChapterRequest
{
	public required ulong comic_id { get; set; }
	public required int chapter { get; set; }
	public required string content { get; set; }
}

public class UpdateComicChapterRequest
{
	public required ulong id { get; set; }
	public required ulong comic_id { get; set; }
	public required int chapter { get; set; }
	public required string content { get; set; }
}
