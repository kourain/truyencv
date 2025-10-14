interface CreateComicCommentRequest {
	comic_id: string;
	comic_chapter_id?: string | null;
	user_id: string;
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
	user_id: string;
	comment: string;
	like: number;
	reply_id: string | null;
	is_rate: boolean;
	rate_star: number | null;
	created_at: string;
	updated_at: string;
}
