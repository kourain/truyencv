namespace TruyenCV.DTO.Response;

public class UserComicBookmarkResponse
{
    public long id { get; set; }
    public long user_id { get; set; }
    public long comic_id { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
