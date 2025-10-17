using TruyenCV;

namespace TruyenCV.DTOs.Response;

public class ComicChapterReadResponse
{
    public string comic_id { get; set; }
    public string comic_slug { get; set; }
    public string comic_title { get; set; }
    public string author_name { get; set; }
    public string chapter_id { get; set; }
    public int chapter_number { get; set; }
    public string chapter_title { get; set; }
    public string content { get; set; }
    public DateTime updated_at { get; set; }
    public int? previous_chapter_number { get; set; }
    public string? previous_chapter_id { get; set; }
    public int? next_chapter_number { get; set; }
    public string? next_chapter_id { get; set; }
    public string? recommended_comic_title { get; set; }
    public string? recommended_comic_slug { get; set; }
    public long monthly_recommendations { get; set; }
    public int month { get; set; }
    public int year { get; set; }
}
