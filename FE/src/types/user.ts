interface CreateUserRequest {
	name: string;
	user_name: string;
	email: string;
	password: string;
}

interface UpdateUserRequest {
	id: number;
	user_name: string;
	name: string;
	email: string;
}

interface AuthTokensResponse {
	access_token: string;
	refresh_token: string;
}

interface LoginResponse extends AuthTokensResponse {
	user: UserResponse;
}

interface BaseResponse {
	message: string;
}
interface LoginRequest {
	email: string;
	password: string;
}

interface RefreshTokenRequest {
	refresh_token: string;
}

interface UserResponse {
	id: number;
	name: string;
	full_name: string;
	email: string;
	created_at: string;
}
