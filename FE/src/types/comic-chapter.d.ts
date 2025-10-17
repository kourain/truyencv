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

interface ComicChapterReadResponse {
	comic_id: string;
	comic_slug: string;
	comic_title: string;
	author_name: string;
	chapter_id: string;
	chapter_number: number;
	chapter_title: string;
	content: string;
	updated_at: string;
	previous_chapter_number?: number | null;
	previous_chapter_id?: string | null;
	next_chapter_number?: number | null;
	next_chapter_id?: string | null;
	recommended_comic_title?: string | null;
	recommended_comic_slug?: string | null;
	monthly_recommendations: number;
	month: number;
	year: number;
}
