interface ComicChapterListItem {
	id: string;
	chapter: number;
	title: string;
	updated_at: string;
	is_locked: boolean;
	key_require: number;
	is_unlocked?: boolean;
}

interface ComicChaptersListResponse {
	comic: {
		id: string;
		slug: string;
		title: string;
		author_name: string;
		cover_url?: string;
	};
	chapters: ComicChapterListItem[];
	total_chapters: number;
	user_last_read_chapter?: number;
}
