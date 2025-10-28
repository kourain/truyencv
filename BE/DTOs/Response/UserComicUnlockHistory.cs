using System;

namespace TruyenCV.DTOs.Response;

public class UserComicUnlockHistoryResponse
{
    public required string id { get; set; }
    public required string user_id { get; set; }
    public required string comic_id { get; set; }
    public required string comic_chapter_id { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
