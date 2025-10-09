import { getHttpClient } from "@helpers/httpClient";

const resource = "/admin/User";

export type UserListParams = {
	offset?: number;
	limit?: number;
};

export const fetchUsers = async (params: UserListParams = {}) => {
	const client = getHttpClient();
	const response = await client.get<UserResponse[]>(resource, { params });

	return response.data;
};

export const fetchUserById = async (id: number) => {
	const client = getHttpClient();
	const response = await client.get<UserResponse>(`${resource}/${id}`);

	return response.data;
};

export type RefreshTokenInfo = {
	id: number;
	token: string;
	expires_at: string;
	is_active: boolean;
	created_at: string;
};

export const fetchUserRefreshTokens = async (userId: number) => {
	const client = getHttpClient();
	const response = await client.get<RefreshTokenInfo[]>(`${resource}/${userId}/refresh-tokens`);

	return response.data;
};

export const revokeUserRefreshToken = async (userId: number, tokenId: number) => {
	const client = getHttpClient();
	const response = await client.delete<BaseResponse>(`${resource}/${userId}/refresh-tokens/${tokenId}`);

	return response.data;
};

export const revokeAllUserRefreshTokens = async (userId: number) => {
	const client = getHttpClient();
	const response = await client.delete<BaseResponse>(`${resource}/${userId}/refresh-tokens`);

	return response.data;
};
