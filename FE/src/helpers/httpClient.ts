import axios, { AxiosError, AxiosInstance, AxiosRequestConfig, AxiosResponse } from "axios";
import { appEnv } from "@const/env";
import { clearAuthTokens, getAccessToken, getRefreshToken, setAuthTokens } from "./authTokens";

const defaultConfig: AxiosRequestConfig = {
  baseURL: appEnv.BACKEND_URL || undefined,
  headers: {
    "Content-Type": "application/json"
  },
  timeout: 15000
};

const redirectToLogin = () => {
  const { pathname, search } = window.location;
  const currentUrl = `${pathname}${search}`;
  const normalizedPath = pathname.toLowerCase();
  const isAdminScope = window.location.hostname === new URL(appEnv.FE_ADMIN).hostname || window.location.hostname.startsWith("localhost");
  const loginBase = isAdminScope ? "/admin/auth/login" : "/user/auth/login";
  const normalizedLoginBase = loginBase.toLowerCase();
  const shouldAppendRedirect = currentUrl && !normalizedPath.startsWith(normalizedLoginBase);
  const loginUrl = shouldAppendRedirect
    ? `${loginBase}?redirect=${encodeURIComponent(currentUrl)}`
    : loginBase;

  window.location.replace(loginUrl);
};

const httpClient: AxiosInstance = axios.create(defaultConfig);

httpClient.interceptors.request.use((config) => {
  const nextConfig = { ...config };

  if (nextConfig.baseURL === undefined && appEnv.BACKEND_URL) {
    nextConfig.baseURL = appEnv.BACKEND_URL;
  }

  const accessToken = getAccessToken();

  const requestUrl = nextConfig.url ?? "";

  if (accessToken && !requestUrl.includes("/auth/login") && !requestUrl.includes("/auth/refresh-token")) {
    nextConfig.headers = nextConfig.headers ?? {};

    if (!nextConfig.headers.Authorization) {
      nextConfig.headers.Authorization = `Bearer ${accessToken}`;
    }
  }

  if (nextConfig.url && !/^https?:\/\//i.test(nextConfig.url)) {
    nextConfig.url = nextConfig.url.startsWith("/") ? nextConfig.url : `/${nextConfig.url}`;
  }

  return nextConfig;
});

httpClient.interceptors.response.use(
  (response: AxiosResponse) => response,
  async (error: AxiosError) => {
    if (error.response) {
      const { status, data, config } = error.response;
      const originalRequest = config ?? error.config;
      if(status ===0 ){
        console.error("[API ERROR] Network Error - Unable to reach the server");
        return Promise.reject(error);
      }
      if (originalRequest?.url?.includes("/auth/refresh-token")) {
        await clearAuthTokens();
        return Promise.reject(error);
      }

      if (status === 401) {
        const retriableRequest = originalRequest as AxiosRequestConfig & { _retry?: boolean };
        const refreshToken = getRefreshToken();
        if (!refreshToken) {
          await clearAuthTokens();
          redirectToLogin();
          console.error("[API ERROR] Missing refresh token for re-authentication");
          return Promise.reject(error);
        }

        retriableRequest._retry = true;

        try {
          const refreshResponse = await httpClient.post<JWT_REFRESH_RESPONSE>(
            "/auth/refresh-token",
            { refresh_token: refreshToken }
          );
          console.log("[API] Tokens refreshed successfully");
          const { access_token, refresh_token, access_token_minutes, refresh_token_days } = refreshResponse.data;
          setAuthTokens(access_token, refresh_token, access_token_minutes, refresh_token_days);

          if (retriableRequest.headers) {
            retriableRequest.headers.Authorization = `Bearer ${access_token}`;
          }

          return httpClient(retriableRequest);
        } catch (refreshError) {
          if (refreshError instanceof AxiosError) {
            if (refreshError.response?.status === 403) {
              console.error("[API ERROR] Refresh token expired or invalid, logging out");
              await clearAuthTokens();
              redirectToLogin();
            } else {
              console.error("[API ERROR] Failed to refresh tokens:", refreshError.response?.data || refreshError.message);
            }
          }
          return Promise.reject(refreshError);
        }
      }

      console.error(`[API ERROR] ${status}:`, data);
    } else if (error.request) {
      console.error("[API ERROR] No response received", error.request);
    } else {
      console.error("[API ERROR] Request setup", error.message);
    }

    return Promise.reject(error);
  }
);

export const getHttpClient = () => httpClient;

export const resolveCdnUrl = (path?: string | null) => {
  if (!path) {
    return appEnv.CDN_URL;
  }

  if (/^https?:\/\//i.test(path)) {
    return path;
  }

  const trimmedPath = path.startsWith("/") ? path.slice(1) : path;
  const base = appEnv.CDN_URL.replace(/\/+$/, "");

  return `${base}/${trimmedPath}`;
};

export type ApiError = AxiosError<{ message?: string; errors?: Record<string, string[]> }>;
