namespace TruyenCV.DTO.Response;

public class UserComicReadHistoryResponse
{
    public ulong id { get; set; }
    public ulong user_id { get; set; }
    public ulong comic_id { get; set; }
    public ulong chapter_id { get; set; }
    public DateTime read_at { get; set; }
    public DateTime updated_at { get; set; }
}
