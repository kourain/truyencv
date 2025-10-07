namespace TruyenCV.DTO.Response;

public class ComicCommentResponse
{
	public long id { get; set; }
	public long comic_id { get; set; }
	public long? comic_chapter_id { get; set; }
	public long user_id { get; set; }
	public string comment { get; set; }
	public int like { get; set; }
	public long? reply_id { get; set; }
	public bool is_rate { get; set; }
	public int? rate_star { get; set; }
	public DateTime created_at { get; set; }
	public DateTime updated_at { get; set; }
}
