interface CreateUserHasRoleRequest {
	role_name: string;
	user_id: string;
	assigned_by: string;
}

interface UpdateUserHasRoleRequest {
	id: string;
	role_name: string;
	user_id: string;
	assigned_by: string;
}

interface UserHasRoleResponse {
	id: string;
	role_name: string;
	user_id: string;
	assigned_by: string;
	created_at: string;
	updated_at: string;
	revoked_at: string | null;
	is_active: boolean;
}
