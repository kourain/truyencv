import { getHttpClient } from "@helpers/httpClient";
import type { ComicResponse, CreateComicRequest, UpdateComicRequest } from "../../types/comic";

export type ComicListParams = {
	offset?: number;
	limit?: number;
	keyword?: string;
	status?: number;
};

const resource = "/admin/Comic";

export const fetchComics = async (params: ComicListParams = {}) => {
	const client = getHttpClient();
	const response = await client.get<ComicResponse[]>(resource, { params });

	return response.data;
};

export const fetchComicById = async (id: number) => {
	const client = getHttpClient();
	const response = await client.get<ComicResponse>(`${resource}/${id}`);

	return response.data;
};

export const fetchComicBySlug = async (slug: string) => {
	const client = getHttpClient();
	const response = await client.get<ComicResponse>(`${resource}/slug/${slug}`);

	return response.data;
};

export const createComic = async (payload: CreateComicRequest) => {
	const client = getHttpClient();
	const response = await client.post<ComicResponse>(resource, payload);

	return response.data;
};

export const updateComic = async (payload: UpdateComicRequest) => {
	const client = getHttpClient();
	const response = await client.put<ComicResponse>(`${resource}/${payload.id}`, payload);

	return response.data;
};

export const deleteComic = async (id: number) => {
	const client = getHttpClient();
	const response = await client.delete<BaseResponse>(`${resource}/${id}`);

	return response.data;
};
