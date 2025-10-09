export interface CreateUserRequest {
	name: string;
	user_name: string;
	email: string;
	password: string;
}

export interface UpdateUserRequest {
	id: number;
	user_name: string;
	name: string;
	email: string;
}

export interface LoginRequest {
	email: string;
	password: string;
}

export interface RefreshTokenRequest {
	refresh_token: string;
}

export interface UserResponse {
	id: number;
	name: string;
	full_name: string;
	email: string;
	created_at: string;
}
