import { getHttpClient } from "@helpers/httpClient";

const resource = "/admin/UserHasRole";

export type UserRoleListParams = {
	offset?: number;
	limit?: number;
};

export const fetchUserRoles = async (params: UserRoleListParams = {}) => {
	const client = getHttpClient();
	const response = await client.get<UserHasRoleResponse[]>(resource, { params });

	return response.data;
};

export const fetchUserRolesByUser = async (userId: string) => {
	const client = getHttpClient();
	const response = await client.get<UserHasRoleResponse[]>(`${resource}/user/${userId}`);

	return response.data;
};

export const fetchUsersByRole = async (roleName: string) => {
	const client = getHttpClient();
	const response = await client.get<UserHasRoleResponse[]>(`${resource}/role/${roleName}`);

	return response.data;
};

export const createUserRole = async (payload: CreateUserHasRoleRequest) => {
	const client = getHttpClient();
	const response = await client.post<UserHasRoleResponse>(resource, payload);

	return response.data;
};

export const updateUserRole = async (payload: UpdateUserHasRoleRequest) => {
	const client = getHttpClient();
	const response = await client.put<UserHasRoleResponse>(`${resource}/${payload.id}`, payload);

	return response.data;
};

export const deleteUserRole = async (id: string) => {
	const client = getHttpClient();
	const response = await client.delete<BaseResponse>(`${resource}/${id}`);

	return response.data;
};
