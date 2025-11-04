namespace TruyenCV.DTOs.Response;

public class ComicResponse
{
	public string id { get; set; }
	public string name { get; set; }
	public string description { get; set; }
	public string slug { get; set; }
	public string author { get; set; }
	public string? embedded_from { get; set; }
	public string? embedded_from_url { get; set; }
	public string? cover_url { get; set; }
	public string? banner_url { get; set; }
	public int chap_count { get; set; }
	public int bookmark_count { get; set; }
	public float rate { get; set; }
    public string main_category { get; set; }
	public ComicStatus status { get; set; }
	public DateTime created_at { get; set; }
	public DateTime updated_at { get; set; }
}

public class ComicSeoResponse
{
	public required string title { get; set; }
	public required string description { get; set; }
	public required IReadOnlyList<string> keywords { get; set; }
	public required string image { get; set; }
}
