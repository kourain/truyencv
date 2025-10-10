namespace TruyenCV.DTO.Response;

public class ComicCommentResponse
{
	public ulong id { get; set; }
	public ulong comic_id { get; set; }
	public ulong? comic_chapter_id { get; set; }
	public ulong user_id { get; set; }
	public string comment { get; set; }
	public int like { get; set; }
	public ulong? reply_id { get; set; }
	public bool is_rate { get; set; }
	public int? rate_star { get; set; }
	public DateTime created_at { get; set; }
	public DateTime updated_at { get; set; }
}
