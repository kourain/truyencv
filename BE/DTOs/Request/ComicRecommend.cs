namespace TruyenCV.DTOs.Request;

public class CreateComicRecommendRequest
{
    public required string comic_id { get; set; }
    public long rcm_count { get; set; } = 0;
    public int month { get; set; }
    public int year { get; set; }
}

public class UpdateComicRecommendRequest
{
    public required string id { get; set; }
    public long rcm_count { get; set; }
    public int? month { get; set; }
    public int? year { get; set; }
}

public class RecommendComicRequest
{
    public required string comic_id { get; set; }
}
