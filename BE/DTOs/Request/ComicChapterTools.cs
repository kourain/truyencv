namespace TruyenCV.DTOs.Request;

public class ConvertChapterRequest
{
    public required string content { get; set; }
}

public class ChapterTtsRequest
{
    public required string content { get; set; }

    public required string reference_audio { get; set; }
}
