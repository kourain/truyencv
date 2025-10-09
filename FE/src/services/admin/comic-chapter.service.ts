import { getHttpClient } from "@helpers/httpClient";

const resource = "/admin/ComicChapter";

export const fetchChaptersByComic = async (comicId: number) => {
	const client = getHttpClient();
	const response = await client.get<ComicChapterResponse[]>(`${resource}/comic/${comicId}`);

	return response.data;
};

export const fetchChapterById = async (id: number) => {
	const client = getHttpClient();
	const response = await client.get<ComicChapterResponse>(`${resource}/${id}`);

	return response.data;
};

export const createComicChapter = async (payload: CreateComicChapterRequest) => {
	const client = getHttpClient();
	const response = await client.post<ComicChapterResponse>(resource, payload);

	return response.data;
};

export const updateComicChapter = async (payload: UpdateComicChapterRequest) => {
	const client = getHttpClient();
	const response = await client.put<ComicChapterResponse>(`${resource}/${payload.id}`, payload);

	return response.data;
};

export const deleteComicChapter = async (id: number) => {
	const client = getHttpClient();
	const response = await client.delete<BaseResponse>(`${resource}/${id}`);

	return response.data;
};
