interface CreateComicChapterRequest {
	comic_id: number;
	chapter: number;
	content: string;
}

interface UpdateComicChapterRequest {
	id: number;
	comic_id: number;
	chapter: number;
	content: string;
}

interface ComicChapterResponse {
	id: number;
	comic_id: number;
	chapter: number;
	content: string;
	created_at: string;
	updated_at: string;
}
