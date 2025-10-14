interface CreateComicCategoryRequest {
	name: string;
}

interface UpdateComicCategoryRequest {
	id: string;
	name: string;
}

interface CreateComicHaveCategoryRequest {
	comic_id: string;
	comic_category_id: string;
}

interface UpdateComicHaveCategoryRequest {
	comic_id: string;
	comic_category_id: string;
	new_comic_id: string;
	new_comic_category_id: string;
}

interface ComicCategoryResponse {
	id: string;
	name: string;
	created_at: string;
	updated_at: string;
}
