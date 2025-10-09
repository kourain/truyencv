import { getHttpClient } from "@helpers/httpClient";

const resource = "/admin/ComicComment";

export type CommentQueryParams =
	| { type: "comic"; id: number }
	| { type: "chapter"; id: number }
	| { type: "user"; id: number }
	| { type: "replies"; id: number };

export const fetchComments = async ({ type, id }: CommentQueryParams) => {
	const client = getHttpClient();
	const response = await client.get<ComicCommentResponse[]>(`${resource}/${type}/${id}`);

	return response.data;
};

export const fetchCommentById = async (id: number) => {
	const client = getHttpClient();
	const response = await client.get<ComicCommentResponse>(`${resource}/${id}`);

	return response.data;
};

export const createComicComment = async (payload: CreateComicCommentRequest) => {
	const client = getHttpClient();
	const response = await client.post<ComicCommentResponse>(resource, payload);

	return response.data;
};

export const updateComicComment = async (payload: UpdateComicCommentRequest) => {
	const client = getHttpClient();
	const response = await client.put<ComicCommentResponse>(`${resource}/${payload.id}`, payload);

	return response.data;
};

export const deleteComicComment = async (id: number) => {
	const client = getHttpClient();
	const response = await client.delete<BaseResponse>(`${resource}/${id}`);

	return response.data;
};
