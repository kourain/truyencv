export interface CreateComicCommentRequest {
	comic_id: number;
	comic_chapter_id?: number | null;
	user_id: number;
	comment: string;
	like?: number;
	reply_id?: number | null;
	is_rate?: boolean;
	rate_star?: number | null;
}

export interface UpdateComicCommentRequest {
	id: number;
	comic_id: number;
	comic_chapter_id?: number | null;
	user_id: number;
	comment: string;
	like: number;
	reply_id?: number | null;
	is_rate: boolean;
	rate_star?: number | null;
}

export interface ComicCommentResponse {
	id: number;
	comic_id: number;
	comic_chapter_id: number | null;
	user_id: number;
	comment: string;
	like: number;
	reply_id: number | null;
	is_rate: boolean;
	rate_star: number | null;
	created_at: string;
	updated_at: string;
}
