using System;
using System.Collections.Generic;

namespace TruyenCV.DTOs.Response;

public class UserHomeHistoryResponse
{
    public long comic_id { get; set; }
    public string comic_title { get; set; } = string.Empty;
    public string? cover_url { get; set; }
    public int last_read_chapter { get; set; }
    public int total_chapters { get; set; }
    public DateTime last_read_at { get; set; }
}

public class UserHomeHighlightedComicResponse
{
    public long comic_id { get; set; }
    public string comic_title { get; set; } = string.Empty;
    public string? cover_url { get; set; }
    public string? short_description { get; set; }
    public int latest_chapter { get; set; }
    public double? average_rating { get; set; }
}

public class UserHomeRankingComicResponse
{
    public long comic_id { get; set; }
    public string comic_title { get; set; } = string.Empty;
    public string? cover_url { get; set; }
    public long total_views { get; set; }
    public long weekly_views { get; set; }
    public long recommendation_score { get; set; }
}

public class UserHomeComicUpdateResponse
{
    public long comic_id { get; set; }
    public string comic_title { get; set; } = string.Empty;
    public string chapter_title { get; set; } = string.Empty;
    public int chapter_number { get; set; }
    public DateTime updated_at { get; set; }
}

public class UserHomeCompletedComicResponse
{
    public long comic_id { get; set; }
    public string comic_title { get; set; } = string.Empty;
    public string? cover_url { get; set; }
    public int total_chapters { get; set; }
    public DateTime completed_at { get; set; }
}

public class UserHomeReviewResponse
{
    public long review_id { get; set; }
    public long comic_id { get; set; }
    public string comic_title { get; set; } = string.Empty;
    public string user_display_name { get; set; } = string.Empty;
    public double rating { get; set; }
    public long liked_count { get; set; }
    public DateTime created_at { get; set; }
    public string? content { get; set; }
}

public class UserHomeResponse
{
    public IEnumerable<UserHomeHistoryResponse> history { get; set; } = Array.Empty<UserHomeHistoryResponse>();
    public IEnumerable<UserHomeHighlightedComicResponse> editor_picks { get; set; } = Array.Empty<UserHomeHighlightedComicResponse>();
    public IEnumerable<UserHomeRankingComicResponse> top_recommended { get; set; } = Array.Empty<UserHomeRankingComicResponse>();
    public IEnumerable<UserHomeRankingComicResponse> top_weekly_reads { get; set; } = Array.Empty<UserHomeRankingComicResponse>();
    public IEnumerable<UserHomeComicUpdateResponse> latest_updates { get; set; } = Array.Empty<UserHomeComicUpdateResponse>();
    public IEnumerable<UserHomeCompletedComicResponse> recently_completed { get; set; } = Array.Empty<UserHomeCompletedComicResponse>();
    public IEnumerable<UserHomeReviewResponse> latest_reviews { get; set; } = Array.Empty<UserHomeReviewResponse>();
}
