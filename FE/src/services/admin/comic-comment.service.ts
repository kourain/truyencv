import { getHttpClient } from "@helpers/httpClient";

const resource = "/admin/ComicComment";

export type CommentQueryParams =
	| { type: "comic"; id: string }
	| { type: "chapter"; id: string }
	| { type: "user"; id: string }
	| { type: "replies"; id: string };

export const fetchComments = async ({ type, id }: CommentQueryParams) => {
	const client = getHttpClient();
	const response = await client.get<ComicCommentResponse[]>(`${resource}/${type}/${id}`);

	return response.data;
};

export const fetchCommentById = async (id: string) => {
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

export const deleteComicComment = async (id: string) => {
	const client = getHttpClient();
	const response = await client.delete<BaseResponse>(`${resource}/${id}`);

	return response.data;
};
