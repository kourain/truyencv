
interface UserHistoryItem {
  comic_id: number;
  comic_title: string;
  comic_slug: string;
  cover_url?: string;
  last_read_chapter: number;
  total_chapters: number;
  last_read_at: string;
};

interface HighlightedComic {
  comic_id: number;
  comic_title: string;
  comic_slug: string;
  cover_url?: string;
  short_description?: string;
  latest_chapter?: number;
  average_rating?: number;
};

interface RankingComic {
  comic_id: number;
  comic_title: string;
  comic_slug: string;
  cover_url?: string;
  total_views: number;
  weekly_views?: number;
  recommendation_score?: number;
};

interface ComicUpdate {
  comic_id: number;
  comic_title: string;
  comic_slug: string;
  chapter_title: string;
  chapter_number: number;
  updated_at: string;
};

interface CompletedComic {
  comic_id: number;
  comic_title: string;
  comic_slug: string;
  cover_url?: string;
  total_chapters: number;
  completed_at: string;
};

interface ReviewSummary {
  review_id: number;
  comic_id: number;
  comic_title: string;
  comic_slug: string;
  user_display_name: string;
  rating: number;
  liked_count: number;
  created_at: string;
  content?: string;
};