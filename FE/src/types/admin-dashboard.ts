import type { ComicResponse } from "./comic";

export type AdminDashboardMetrics = {
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

export type AdminDashboardCategorySummary = ComicCategoryResponse & {
	comics_count?: number;
};

export type AdminDashboardOverview = {
	metrics: AdminDashboardMetrics;
	top_comics: ComicResponse[];
	recent_users: UserResponse[];
	category_highlights: AdminDashboardCategorySummary[];
	generated_at: string;
	is_mock?: boolean;
};
