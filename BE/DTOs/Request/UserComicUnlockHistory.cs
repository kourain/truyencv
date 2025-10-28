namespace TruyenCV.DTOs.Request;

public class CreateUserComicUnlockHistoryRequest
{
    public required string user_id { get; set; }
    public required string comic_id { get; set; }
    public required string comic_chapter_id { get; set; }
}

public class UnlockComicChapterRequest
{
    public required string comic_id { get; set; }
    public required string comic_chapter_id { get; set; }
}
