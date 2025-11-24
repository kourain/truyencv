using System;

namespace TruyenCV.DTOs.Response;

public class UserUseKeyHistoryResponse
{
    public required string id { get; set; }
    public required string user_id { get; set; }
    public long key { get; set; }
    public HistoryStatus status { get; set; }
    public string? chapter_id { get; set; }
    public string? comic_name { get; set; }
    public int? chapter_number { get; set; }
    public string? note { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
