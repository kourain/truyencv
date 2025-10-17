namespace TruyenCV.DTOs.Response;

public class ComicRecommendResponse
{
    public string id { get; set; }
    public string comic_id { get; set; }
    public long rcm_count { get; set; }
    public int month { get; set; }
    public int year { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
