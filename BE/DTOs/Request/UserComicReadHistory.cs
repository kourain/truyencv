namespace TruyenCV.DTOs.Request;

public class UpsertUserComicReadHistoryRequest
{
    public required string comic_id { get; set; }
    public required string chapter_id { get; set; }
}
