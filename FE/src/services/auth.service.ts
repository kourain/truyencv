import { getHttpClient } from "@helpers/httpClient";
import { clearAuthTokens, getRefreshToken, setAuthTokens } from "@helpers/authTokens";

import type { AxiosRequestConfig } from "axios";

type RefreshTokenPayload = RefreshTokenRequest;

type MessageResponse = BaseResponse;

export const login = async (payload: LoginRequest, config?: AxiosRequestConfig) => {
	const client = getHttpClient();
	const response = await client.post<LoginResponse>("/auth/login", payload, config);
	const { access_token, refresh_token } = response.data;

	setAuthTokens(access_token, refresh_token);

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

	setAuthTokens(response.data.access_token, response.data.refresh_token);

	return response.data;
};

export const logout = async (payload?: RefreshTokenPayload, config?: AxiosRequestConfig) => {
	const client = getHttpClient();
	const refresh_token = payload?.refresh_token ?? getRefreshToken();

	if (!refresh_token) {
		clearAuthTokens();
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
		clearAuthTokens();
	}
};

export const logoutAll = async (config?: AxiosRequestConfig) => {
	const client = getHttpClient();

	try {
		const response = await client.post<MessageResponse>("/auth/logout-all", null, config);

		return response.data;
	} finally {
		clearAuthTokens();
	}
};
