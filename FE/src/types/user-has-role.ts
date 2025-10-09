export interface CreateUserHasRoleRequest {
	role_name: string;
	user_id: number;
	assigned_by: number;
}

export interface UpdateUserHasRoleRequest {
	id: number;
	role_name: string;
	user_id: number;
	assigned_by: number;
}

export interface UserHasRoleResponse {
	id: number;
	role_name: string;
	user_id: number;
	assigned_by: number;
	created_at: string;
	updated_at: string;
}
