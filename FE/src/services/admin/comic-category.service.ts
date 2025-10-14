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

const fallbackCategoryNames = ["Huyền huyễn", "Tiên hiệp", "Đô thị", "Khoa huyễn", "Kiếm hiệp", "Hài hước"];

const createMockCategories = (limit?: number): ComicCategoryResponse[] => {
	const now = Date.now();
	const total = Math.min(Math.max(1, limit ?? fallbackCategoryNames.length), fallbackCategoryNames.length);

	return Array.from({ length: total }).map((_, index) => ({
		id: String(index + 1),
		name: fallbackCategoryNames[index],
		created_at: new Date(now - index * 86_400_000).toISOString(),
		updated_at: new Date(now - index * 43_200_000).toISOString()
	}));
};

export const fetchComicCategoriesWithFallback = async (params: CategoryListParams = {}) => {
	try {
		const data = await fetchComicCategories(params);
		return { data, isMock: false };
	} catch (error) {
		if (process.env.NODE_ENV !== "production") {
			console.warn("[ADMIN][ComicCategoryService] Sử dụng dữ liệu mock do lỗi API", error);
		}

		return {
			data: createMockCategories(params.limit),
			isMock: true
		};
	}
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

export const deleteComicCategory = async (id: string) => {
	const client = getHttpClient();
	const response = await client.delete<BaseResponse>(`${resource}/${id}`);

	return response.data;
};
