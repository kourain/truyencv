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
	access_token_minutes: number;
	refresh_token_days: number;
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
	access_token_minutes: number;
	refresh_token_days: number;
}

interface LoginResponse extends AuthTokensResponse {
	user: UserResponse;
	access_token_minutes: number;
	refresh_token_days: number;
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

interface UserProfileResponse {
  id: number;
  name: string;
  email: string;
  phone: string;
  avatar: string;
  created_at: string;
  updated_at: string;
  email_verified_at: string | null;
  banned_at: string | null;
  is_banned: boolean;
  read_comic_count: number;
  read_chapter_count: number;
  bookmark_count: number;
  coin: number;
  roles: string[];
  permissions: string[];
};

interface ChangePasswordPayload {
  current_password: string;
  new_password: string;
};