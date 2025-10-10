namespace TruyenCV.DTO.Request;

public class UpsertUserComicReadHistoryRequest
{
    public required ulong comic_id { get; set; }
    public required ulong chapter_id { get; set; }
}
