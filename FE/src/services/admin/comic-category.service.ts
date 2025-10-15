import { CategoryType } from "@const/enum/category-type";
import { getHttpClient } from "@helpers/httpClient";
import type {
	ComicCategoryResponse,
	CreateComicCategoryRequest,
	UpdateComicCategoryRequest
} from "../../types/comic-category";

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

const fallbackCategories: Array<{ name: string; category_type: CategoryType }> = [
	{ name: "Huyền huyễn", category_type: CategoryType.Genre },
	{ name: "Tiên hiệp", category_type: CategoryType.Genre },
	{ name: "Đô thị", category_type: CategoryType.Genre },
	{ name: "Khoa huyễn", category_type: CategoryType.Genre },
	{ name: "Kiếm hiệp", category_type: CategoryType.Genre },
	{ name: "Hài hước", category_type: CategoryType.Genre }
];

const createMockCategories = (limit?: number): ComicCategoryResponse[] => {
	const now = Date.now();
	const total = Math.min(Math.max(1, limit ?? fallbackCategories.length), fallbackCategories.length);

	return Array.from({ length: total }).map((_, index) => ({
		id: String(index + 1),
		name: fallbackCategories[index].name,
		category_type: fallbackCategories[index].category_type,
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
