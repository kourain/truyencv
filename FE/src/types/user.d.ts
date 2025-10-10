interface CreateUserRequest {
	name: string;
	user_name: string;
	email: string;
	password: string;
	phone: string;
}

interface RegisterRequest {
	name: string;
	user_name: string;
	email: string;
	password: string;
	phone: string;
}

interface RegisterResponse extends AuthTokensResponse {
  user: UserResponse;
	message?: string;
}
interface UpdateUserRequest {
	id: number;
	user_name: string;
	name: string;
	email: string;
	phone: string;
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

interface RequestPasswordResetRequest {
	email: string;
}

interface ConfirmPasswordResetRequest {
	email: string;
	otp: string;
	new_password: string;
}
