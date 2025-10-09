import { getHttpClient } from "@helpers/httpClient";

const resource = "/admin/ComicCategory";

export type CategoryListParams = {
	offset?: number;
	limit?: number;
};

export const fetchComicCategories = async (params: CategoryListParams = {}) => {
	const client = getHttpClient();
	const response = await client.get<ComicCategoryResponse[]>(resource, { params });

	return response.data;
};

export const fetchAllComicCategories = async () => {
	const client = getHttpClient();
	const response = await client.get<ComicCategoryResponse[]>(`${resource}/all`);

	return response.data;
};

export const createComicCategory = async (payload: CreateComicCategoryRequest) => {
	const client = getHttpClient();
	const response = await client.post<ComicCategoryResponse>(resource, payload);

	return response.data;
};

export const updateComicCategory = async (payload: UpdateComicCategoryRequest) => {
	const client = getHttpClient();
	const response = await client.put<ComicCategoryResponse>(`${resource}/${payload.id}`, payload);

	return response.data;
};

export const deleteComicCategory = async (id: number) => {
	const client = getHttpClient();
	const response = await client.delete<BaseResponse>(`${resource}/${id}`);

	return response.data;
};
