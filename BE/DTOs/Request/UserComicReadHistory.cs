namespace TruyenCV.DTO.Request;

public class UpsertUserComicReadHistoryRequest
{
    public required long comic_id { get; set; }
    public required long chapter_id { get; set; }
}
