namespace TruyenCV.DTO.Response;

public class UserComicBookmarkResponse
{
    public ulong id { get; set; }
    public ulong user_id { get; set; }
    public ulong comic_id { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
