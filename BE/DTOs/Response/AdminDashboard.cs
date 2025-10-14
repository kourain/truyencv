using System;
using System.Collections.Generic;

namespace TruyenCV.DTO.Response;

public class AdminDashboardMetricsResponse
{
	public int total_comics { get; set; }
	public int continuing_comics { get; set; }
	public int completed_comics { get; set; }
	public int total_users { get; set; }
	public int new_users_7_days { get; set; }
	public int categories_count { get; set; }
	public long total_chapters { get; set; }
	public long total_comments { get; set; }
	public long total_bookmarks { get; set; }
	public int active_admins { get; set; }
}

public class AdminDashboardCategorySummaryResponse
{
	public long id { get; set; }
	public string name { get; set; }
	public int comics_count { get; set; }
	public DateTime created_at { get; set; }
	public DateTime updated_at { get; set; }
}

public class AdminDashboardOverviewResponse
{
	public AdminDashboardMetricsResponse metrics { get; set; } = new();
	public IEnumerable<ComicResponse> top_comics { get; set; } = [];
	public IEnumerable<UserResponse> recent_users { get; set; } = [];
	public IEnumerable<AdminDashboardCategorySummaryResponse> category_highlights { get; set; } = [];
	public DateTime generated_at { get; set; } = DateTime.UtcNow;
	public bool is_mock { get; set; } = false;
}
