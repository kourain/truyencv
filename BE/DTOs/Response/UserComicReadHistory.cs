namespace TruyenCV.DTO.Response;

public class UserComicReadHistoryResponse
{
    public long id { get; set; }
    public long user_id { get; set; }
    public long comic_id { get; set; }
    public long chapter_id { get; set; }
    public DateTime read_at { get; set; }
    public DateTime updated_at { get; set; }
}
