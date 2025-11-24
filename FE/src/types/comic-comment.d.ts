interface CreateComicCommentRequest {
	comic_id: string;
	comic_chapter_id?: string | null;
	comment: string;
	like?: number;
	reply_id?: string | null;
	is_rate?: boolean;
	rate_star?: number | null;
}

interface UpdateComicCommentRequest {
	id: string;
	comic_id: string;
	comic_chapter_id?: string | null;
	user_id: string;
	comment: string;
	like: number;
	reply_id?: string | null;
	is_rate: boolean;
	rate_star?: number | null;
}

interface ComicCommentResponse {
	id: string;
	comic_id: string;
	comic_chapter_id: string | null;
	comic_name?: string | null;
	chapter_number?: number | null;
	user_id: string;
	comment: string;
	like: number;
	reply_id: string | null;
	is_rate: boolean;
	rate_star: number | null;
	created_at: string;
	updated_at: string;
}

interface RecommendStatusResponse {
  comic_id: string | number;
  has_recommended: boolean;
};

interface ComicRecommendResponse {
	id: string;
	comic_id: string;
	rcm_count: number;
	month: number;
	year: number;
	created_at: string;
	updated_at: string;
};
