import { getHttpClient } from "@helpers/httpClient";
import type { ComicResponse } from "../../types/comic";

const resource = "/admin/ComicHaveCategory";

export const fetchCategoriesOfComic = async (comicId: string) => {
	const client = getHttpClient();
	const response = await client.get<ComicCategoryResponse[]>(`${resource}/comic/${comicId}/categories`);

	return response.data;
};

export const fetchComicsOfCategory = async (categoryId: string) => {
	const client = getHttpClient();
	const response = await client.get<ComicResponse[]>(`${resource}/category/${categoryId}/comics`);

	return response.data;
};

export const addComicToCategory = async (payload: CreateComicHaveCategoryRequest) => {
	const client = getHttpClient();
	const response = await client.post<BaseResponse>(resource, payload);

	return response.data;
};

export const removeComicFromCategory = async (comicId: string, categoryId: string) => {
	const client = getHttpClient();
	const response = await client.delete<BaseResponse>(`${resource}/comic/${comicId}/category/${categoryId}`);

	return response.data;
};
