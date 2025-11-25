namespace TruyenCV.DTOs.Response;

public class UserBookmarkWithComicDetailResponse
{
    public string id { get; set; }
    public string comic_id { get; set; }
    public string comic_title { get; set; }
    public string comic_slug { get; set; }
    public string comic_cover_url { get; set; }
    public int? user_last_read_chapter { get; set; }
    public int latest_chapter_number { get; set; }
    public DateTime bookmarked_at { get; set; }
}
