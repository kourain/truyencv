import { getHttpClient } from "@helpers/httpClient";
import { ComicStatus } from "@const/comic-status";
import { ComicResponse, CreateComicRequest, UpdateComicRequest } from "../../types/comic";

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

const createMockComics = (limit = 8): ComicResponse[] => {
	const now = Date.now();
	const total = Math.max(1, limit);

	return Array.from({ length: total }).map((_, index) => ({
		id: index + 1,
		name: `Truyện demo ${index + 1}`,
		description: "Nội dung demo cho mục dashboard khi API chưa sẵn sàng.",
		slug: `truyen-demo-${index + 1}`,
		author: index % 2 === 0 ? "Lam Nguyệt" : "Hàn Vũ",
		embedded_from: null,
		embedded_from_url: null,
		chap_count: 40 + index * 5,
		bookmark_count: 10 + index * 3,
		rate: 3.5 + (index % 3) * 0.4,
		status: index % 4 === 0 ? ComicStatus.Completed : ComicStatus.Continuing,
		created_at: new Date(now - index * 86_400_000).toISOString(),
		updated_at: new Date(now - index * 43_200_000).toISOString()
	}));
};

export const fetchComicsWithFallback = async (params: ComicListParams = {}) => {
	try {
		const data = await fetchComics(params);
		return { data, isMock: false };
	} catch (error) {
		if (process.env.NODE_ENV !== "production") {
			console.warn("[ADMIN][ComicService] Sử dụng dữ liệu mock do lỗi API", error);
		}

		return {
			data: createMockComics(params.limit ?? 8),
			isMock: true
		};
	}
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
