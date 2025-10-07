namespace TruyenCV.DTO.Response;

public class ComicResponse
{
	public long id { get; set; }
	public string name { get; set; }
	public string description { get; set; }
	public string slug { get; set; }
	public string author { get; set; }
	public string? embedded_from { get; set; }
	public string? embedded_from_url { get; set; }
	public int chap_count { get; set; }
	public float rate { get; set; }
	public ComicStatus status { get; set; }
	public DateTime created_at { get; set; }
	public DateTime updated_at { get; set; }
}
