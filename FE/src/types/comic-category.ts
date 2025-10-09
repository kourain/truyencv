export interface CreateComicCategoryRequest {
	name: string;
}

export interface UpdateComicCategoryRequest {
	id: number;
	name: string;
}

export interface CreateComicHaveCategoryRequest {
	comic_id: number;
	comic_category_id: number;
}

export interface UpdateComicHaveCategoryRequest {
	comic_id: number;
	comic_category_id: number;
	new_comic_id: number;
	new_comic_category_id: number;
}

export interface ComicCategoryResponse {
	id: number;
	name: string;
	created_at: string;
	updated_at: string;
}
