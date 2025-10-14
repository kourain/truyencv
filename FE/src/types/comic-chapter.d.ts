interface CreateComicChapterRequest {
	comic_id: string;
	chapter: number;
	content: string;
}

interface UpdateComicChapterRequest {
	id: string;
	comic_id: string;
	chapter: number;
	content: string;
}

interface ComicChapterResponse {
	id: string;
	comic_id: string;
	chapter: number;
	content: string;
	created_at: string;
	updated_at: string;
}
