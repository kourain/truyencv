using System;
using System.Collections.Generic;

namespace TruyenCV.DTOs.Response;

public class SearchComicResponse
{
	public string id { get; set; } = string.Empty;
	public string name { get; set; } = string.Empty;
	public string slug { get; set; } = string.Empty;
	public string? cover_url { get; set; }
	public string? author { get; set; }
	public string? description { get; set; }
	public string? main_category { get; set; }
	public int chap_count { get; set; }
	public float rate { get; set; }
	public int rate_count { get; set; }
	public double match_score { get; set; } // 0.0 - 1.0, similarity score
}
