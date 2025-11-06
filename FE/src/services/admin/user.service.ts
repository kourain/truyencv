import { getHttpClient } from "@helpers/httpClient";

const resource = "/admin/User";

export type UserListParams = {
	offset?: number;
	limit?: number;
	keyword?: string;
};

export const fetchUsers = async (params: UserListParams = {}) => {
	const client = getHttpClient();
	const response = await client.get<UserResponse[]>(resource, { params });

	return response.data;
};

const createMockUsers = (limit = 6): UserResponse[] => {
	const now = Date.now();
	const total = Math.max(1, limit);

	return Array.from({ length: total }).map((_, index) => ({
		id: String(index + 1_000),
		name: `Người dùng ${index + 1}`,
		full_name: `Quản trị viên ${index + 1}`,
		email: `admin${index + 1}@example.com`,
		created_at: new Date(now - index * 28_800_000).toISOString()
	}));
};

const filterMockUsers = (users: UserResponse[], keyword?: string) => {
	if (!keyword) {
		return users;
	}

	const normalized = keyword.trim().toLowerCase();

	return users.filter((user) => {
		const email = user.email?.toLowerCase() ?? "";
		const id = user.id.toLowerCase();
		const name = (user.full_name ?? user.name ?? "").toLowerCase();

		return (
			email.includes(normalized) ||
			id === normalized ||
			name.includes(normalized)
		);
	});
};

export const fetchUsersWithFallback = async (params: UserListParams = {}) => {
	try {
		const data = await fetchUsers(params);
		return { data, isMock: false };
	} catch (error) {
		if (process.env.NODE_ENV !== "production") {
			console.warn("[ADMIN][UserService] Sử dụng dữ liệu mock do lỗi API", error);
		}

		const mock = createMockUsers(params.limit ?? 6);
		const filtered = filterMockUsers(mock, params.keyword);

		return {
			data: filtered.slice(0, params.limit ?? filtered.length),
			isMock: true
		};
	}
};

export const fetchUserById = async (id: string) => {
	const client = getHttpClient();
	const response = await client.get<UserResponse>(`${resource}/${id}`);

	return response.data;
};

export type RefreshTokenInfo = {
	id: string;
	token: string;
	expires_at: string;
	is_active: boolean;
	created_at: string;
};

export const fetchUserRefreshTokens = async (userId: string) => {
	const client = getHttpClient();
	const response = await client.get<RefreshTokenInfo[]>(`${resource}/${userId}/refresh-tokens`);

	return response.data;
};

export const revokeUserRefreshToken = async (userId: string, tokenId: string) => {
	const client = getHttpClient();
	const response = await client.delete<BaseResponse>(`${resource}/${userId}/refresh-tokens/${tokenId}`);

	return response.data;
};

export const revokeAllUserRefreshTokens = async (userId: string) => {
	const client = getHttpClient();
	const response = await client.delete<BaseResponse>(`${resource}/${userId}/refresh-tokens`);

	return response.data;
};
