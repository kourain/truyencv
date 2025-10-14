namespace TruyenCV.DTO.Response;

public class UserComicBookmarkResponse
{
    public string id { get; set; }
    public string user_id { get; set; }
    public string comic_id { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
