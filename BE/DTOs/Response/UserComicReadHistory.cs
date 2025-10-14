namespace TruyenCV.DTO.Response;

public class UserComicReadHistoryResponse
{
    public string id { get; set; }
    public string user_id { get; set; }
    public string comic_id { get; set; }
    public string chapter_id { get; set; }
    public DateTime read_at { get; set; }
    public DateTime updated_at { get; set; }
}
