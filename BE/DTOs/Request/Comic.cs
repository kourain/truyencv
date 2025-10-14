namespace TruyenCV.DTO.Request;

// Request DTOs for creating and updating comics
public class CreateComicRequest
{
	public required string name { get; set; }
	public required string description { get; set; }
	public required string slug { get; set; }
	public required string author { get; set; }
	public string? embedded_from { get; set; }
	public string? embedded_from_url { get; set; }
	public int chap_count { get; set; } = 0;
	public float rate { get; set; } = 0;
	public ComicStatus status { get; set; } = ComicStatus.Continuing;
}

public class UpdateComicRequest
{
	public required string id { get; set; }
	public required string name { get; set; }
	public required string description { get; set; }
	public required string slug { get; set; }
	public required string author { get; set; }
	public string? embedded_from { get; set; }
	public string? embedded_from_url { get; set; }
	public int chap_count { get; set; }
	public float rate { get; set; }
	public ComicStatus status { get; set; }
}
