import type { ComicResponse } from "./comic";

interface AdminDashboardMetrics {
	total_comics: number;
	continuing_comics: number;
	completed_comics: number;
	total_users: number;
	new_users_7_days: number;
	categories_count: number;
	total_chapters: string;
	total_comments: string;
	total_bookmarks: string;
	active_admins: number;
};

interface AdminDashboardCategorySummary extends ComicCategoryResponse {
	comics_count?: number;
};

interface AdminDashboardOverview {
	metrics: AdminDashboardMetrics;
	top_comics: ComicResponse[];
	recent_users: UserResponse[];
	category_highlights: AdminDashboardCategorySummary[];
	generated_at: string;
	is_mock?: boolean;
};
