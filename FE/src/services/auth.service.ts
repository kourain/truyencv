import { getHttpClient } from "@helpers/httpClient";
import { clearAuthTokens, getRefreshToken, setAuthTokens } from "@helpers/authTokens";

import type { AxiosRequestConfig } from "axios";

type RefreshTokenPayload = RefreshTokenRequest;

type MessageResponse = BaseResponse;

export const register = async (payload: RegisterRequest, config?: AxiosRequestConfig) => {
	const client = getHttpClient();
	const response = await client.post<RegisterResponse>("/auth/register", payload, config);
	const { access_token, refresh_token, access_token_minutes, refresh_token_days } = response.data;
	setAuthTokens(access_token, refresh_token, access_token_minutes, refresh_token_days);
	return response.data;
};

export const login = async (payload: LoginRequest, config?: AxiosRequestConfig) => {
	const client = getHttpClient();
	const response = await client.post<LoginResponse>("/auth/login", payload, config);
	const { access_token, refresh_token, access_token_minutes, refresh_token_days } = response.data;

	setAuthTokens(access_token, refresh_token, access_token_minutes, refresh_token_days);

	return response.data;
};

export const loginWithFirebase = async (payload: FirebaseLoginRequest, config?: AxiosRequestConfig) => {
	const client = getHttpClient();
	const response = await client.post<LoginResponse>("/auth/firebase-login", payload, config);
	const { access_token, refresh_token, access_token_minutes, refresh_token_days } = response.data;

	setAuthTokens(access_token, refresh_token, access_token_minutes, refresh_token_days);

	return response.data;
};

export const refreshTokens = async (payload?: RefreshTokenPayload, config?: AxiosRequestConfig) => {
	const client = getHttpClient();
	const refresh_token = payload?.refresh_token ?? getRefreshToken();

	if (!refresh_token) {
		throw new Error("Không tìm thấy refresh token để làm mới phiên làm việc");
	}

	const response = await client.post<AuthTokensResponse>(
		"/auth/refresh-token",
		{ refresh_token },
		config
	);

	setAuthTokens(response.data.access_token, response.data.refresh_token, response.data.access_token_minutes, response.data.refresh_token_days);

	return response.data;
};

export const logout = async (payload?: RefreshTokenPayload, config?: AxiosRequestConfig) => {
	const client = getHttpClient();
	const refresh_token = payload?.refresh_token ?? getRefreshToken();

	if (!refresh_token) {
		await clearAuthTokens({ from_logout: true });
		return { message: "Đăng xuất thành công" } satisfies MessageResponse;
	}

	try {
		const response = await client.post<MessageResponse>(
			"/auth/logout",
			{ refresh_token },
			config
		);

		return response.data;
	} finally {
		await clearAuthTokens({ from_logout: true });
	}
};

export const logoutAll = async (config?: AxiosRequestConfig) => {
	const client = getHttpClient();
	const refresh_token = getRefreshToken();

	if (!refresh_token) {
		throw new Error("Không tìm thấy refresh token để đăng xuất các thiết bị khác");
	}

	const response = await client.post<MessageResponse>(
		"/auth/logout-all",
		{ refresh_token },
		config
	);

	return response.data;
};

export const requestPasswordReset = async (payload: RequestPasswordResetRequest, config?: AxiosRequestConfig) => {
	const client = getHttpClient();
	const response = await client.post<BaseResponse>("/auth/password-reset/request", payload, config);
	return response.data;
};

export const confirmPasswordReset = async (payload: ConfirmPasswordResetRequest, config?: AxiosRequestConfig) => {
	const client = getHttpClient();
	const response = await client.post<BaseResponse>("/auth/password-reset/confirm", payload, config);
	return response.data;
};
