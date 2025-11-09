namespace TruyenCV.DTOs.Request;

// Request DTOs for creating and updating comics
public class CreateComicRequest
{
	public required string name { get; set; }
	public required string description { get; set; }
	public string? slug { get; set; } // Optional - backend will auto-generate if null
	public required string author { get; set; }
	public string? embedded_from { get; set; }
	public string? embedded_from_url { get; set; }
	public string? cover_url { get; set; }
	public string? banner_url { get; set; }
    public long main_category_id { get; set; } = 1001; // Default to first category (Tiên Hiệp)
	public List<long>? category_ids { get; set; } // Optional - additional categories to add to comic
	// Note: chap_count and rate are not allowed in Create - they are auto-calculated
	public ComicStatus status { get; set; } = ComicStatus.Continuing;
}

public class UpdateComicRequest
{
	public required string id { get; set; }
	public required string name { get; set; }
	public required string description { get; set; }
	public required string? slug { get; set; }
	public required string author { get; set; }
	public string? embedded_from { get; set; }
	public string? embedded_from_url { get; set; }
	public string? cover_url { get; set; }
	public string? banner_url { get; set; }
    public long main_category_id { get; set; }
    public int chap_count { get; set; }
    public float rate { get; set; }
	public ComicStatus status { get; set; }
}
