interface CreateComicCategoryRequest {
	name: string;
}

interface UpdateComicCategoryRequest {
	id: number;
	name: string;
}

interface CreateComicHaveCategoryRequest {
	comic_id: number;
	comic_category_id: number;
}

interface UpdateComicHaveCategoryRequest {
	comic_id: number;
	comic_category_id: number;
	new_comic_id: number;
	new_comic_category_id: number;
}

interface ComicCategoryResponse {
	id: number;
	name: string;
	created_at: string;
	updated_at: string;
}
