namespace TruyenCV.DTO.Response;

public class ComicChapterResponse
{
	public string id { get; set; }
	public string comic_id { get; set; }
	public int chapter { get; set; }
	public string content { get; set; }
	public DateTime created_at { get; set; }
	public DateTime updated_at { get; set; }
}
