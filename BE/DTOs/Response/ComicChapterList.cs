using System;
using System.Collections.Generic;

namespace TruyenCV.DTOs.Response;

public class ComicChapterListComicResponse
{
    public string id { get; set; } = string.Empty;
    public string slug { get; set; } = string.Empty;
    public string title { get; set; } = string.Empty;
    public string author_name { get; set; } = string.Empty;
    public string? cover_url { get; set; }
}

public class ComicChapterListItemResponse
{
    public string id { get; set; } = string.Empty;
    public int chapter { get; set; }
    public string title { get; set; } = string.Empty;
    public DateTime updated_at { get; set; }
    public bool is_locked { get; set; }
    public int key_require { get; set; }
    public bool is_unlocked { get; set; }
}

public class ComicChaptersListResponse
{
    public ComicChapterListComicResponse comic { get; set; } = new();
    public IReadOnlyList<ComicChapterListItemResponse> chapters { get; set; } = [];
    public int total_chapters { get; set; }
    public int? user_last_read_chapter { get; set; }
}
