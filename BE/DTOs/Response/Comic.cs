using System;
using System.Collections.Generic;

namespace TruyenCV.DTOs.Response;

public class ComicResponse
{
    public string id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public string slug { get; set; }
    public string author { get; set; }
    public string? embedded_from { get; set; }
    public string? embedded_from_url { get; set; }
    public string? cover_url { get; set; }
    public string? banner_url { get; set; }
    public int chap_count { get; set; }
    public int bookmark_count { get; set; }
    public float rate { get; set; }
    public int rate_count { get; set; }
    public string main_category { get; set; }
    public ComicStatus status { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}

public class ComicSeoResponse
{
    public required string title { get; set; }
    public required string description { get; set; }
    public required IReadOnlyList<string> keywords { get; set; }
    public required string image { get; set; }
}

public class ComicDetailCategoryResponse
{
    public string id { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
}

public class ComicDetailChapterResponse
{
    public string id { get; set; } = string.Empty;
    public int number { get; set; }
    public string title { get; set; } = string.Empty;
    public DateTime released_at { get; set; }
}

public class ComicDetailAdvertisementResponse
{
    public string id { get; set; } = string.Empty;
    public string image_url { get; set; } = string.Empty;
    public string href { get; set; } = string.Empty;
    public string label { get; set; } = string.Empty;
    public string? description { get; set; }
}

public class ComicDetailAdvertisementsResponse
{
    public ComicDetailAdvertisementResponse? primary { get; set; }
    public ComicDetailAdvertisementResponse? secondary { get; set; }
    public ComicDetailAdvertisementResponse? tertiary { get; set; }
}

public class ComicDetailReviewResponse
{
    public string id { get; set; } = string.Empty;
    public string user_display_name { get; set; } = string.Empty;
    public double rating { get; set; }
    public string comment { get; set; } = string.Empty;
    public DateTime created_at { get; set; }
}

public class ComicDetailDiscussionResponse
{
    public string id { get; set; } = string.Empty;
    public string user_display_name { get; set; } = string.Empty;
    public string message { get; set; } = string.Empty;
    public DateTime created_at { get; set; }
}

public class ComicDetailRelatedComicResponse
{
    public string id { get; set; } = string.Empty;
    public string slug { get; set; } = string.Empty;
    public string title { get; set; } = string.Empty;
    public string? cover_url { get; set; }
    public int? latest_chapter { get; set; }
}

public class ComicDetailComicResponse
{
    public string id { get; set; } = string.Empty;
    public string slug { get; set; } = string.Empty;
    public string title { get; set; } = string.Empty;
    public string synopsis { get; set; } = string.Empty;
    public string author_name { get; set; } = string.Empty;
    public string? cover_url { get; set; }
    public string? banner_url { get; set; }
    public double rate { get; set; }
    public int rate_count { get; set; }
    public int bookmark_count { get; set; }
    public int weekly_chapter_count { get; set; }
    public int monthly_recommendations { get; set; }
    public int? user_last_read_chapter { get; set; }
    public IReadOnlyList<ComicDetailCategoryResponse> categories { get; set; } = [];
}

public class ComicDetailResponse
{
    public ComicDetailComicResponse comic { get; set; } = new();
    public IReadOnlyList<ComicDetailChapterResponse> latest_chapters { get; set; } = [];
    public ComicDetailAdvertisementsResponse advertisements { get; set; } = new();
    public string introduction { get; set; } = string.Empty;
    public IReadOnlyList<ComicDetailRelatedComicResponse> related_by_author { get; set; } = [];
    public IReadOnlyList<ComicDetailReviewResponse> reviews { get; set; } = [];
    public IReadOnlyList<ComicDetailDiscussionResponse> discussions { get; set; } = [];
    public IReadOnlyList<string> highlights { get; set; } = [];
}
