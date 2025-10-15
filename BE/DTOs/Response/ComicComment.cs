namespace TruyenCV.DTOs.Response;

public class ComicCommentResponse
{
	public string id { get; set; }
	public string comic_id { get; set; }
	public string? comic_chapter_id { get; set; }
	public string user_id { get; set; }
	public string comment { get; set; }
	public int like { get; set; }
	public string? reply_id { get; set; }
	public bool is_rate { get; set; }
	public int? rate_star { get; set; }
	public DateTime created_at { get; set; }
	public DateTime updated_at { get; set; }
}
