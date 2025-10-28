import axios, { AxiosError, AxiosInstance, AxiosRequestConfig, AxiosResponse } from "axios";
import { appEnv, isBrowser } from "@const/env";
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

const getHeaderValue = (headers: AxiosResponse["headers"], headerName: string) => {
  if (!headers) {
    return undefined;
  }

  const targetKey = Object.keys(headers).find((key) => key.toLowerCase() === headerName.toLowerCase());
  return targetKey ? headers[targetKey] : undefined;
};

const extractAccessToken = (headers: AxiosResponse["headers"]) => {
  const authHeader = getHeaderValue(headers, "authorization");
  if (typeof authHeader === "string") {
    const [, token] = authHeader.split(/Bearer\s+/i);
    return token?.trim() ?? null;
  }

  const accessHeader = getHeaderValue(headers, "access_token");
  return typeof accessHeader === "string" ? accessHeader.trim() : null;
};

const extractRefreshToken = (headers: AxiosResponse["headers"], fallback?: string | null) => {
  const headerValue =
    getHeaderValue(headers, "x-refresh-token") ??
    getHeaderValue(headers, "refresh-token") ??
    getHeaderValue(headers, "refresh_token");

  if (typeof headerValue === "string" && headerValue.trim()) {
    return headerValue.trim();
  }

  return fallback ?? null;
};

httpClient.interceptors.response.use(
  (response: AxiosResponse) => {
    if (isBrowser) {
      const refreshToken = getRefreshToken();
      const newAccessToken = extractAccessToken(response.headers);
      const responseData = response?.data as Partial<AuthTokensResponse> | undefined;
      const bodyAccessToken = responseData?.access_token;
      const bodyRefreshToken = responseData?.refresh_token;

      const updatedAccessToken = newAccessToken ?? bodyAccessToken ?? null;
      const updatedRefreshToken = extractRefreshToken(response.headers, bodyRefreshToken ?? refreshToken ?? null);

      if (updatedAccessToken && updatedRefreshToken) {
        const accessExpiryMinutes = responseData?.access_token_minutes;
        const refreshExpiryDays = responseData?.refresh_token_days;
        const hasExpiryMeta =
          typeof accessExpiryMinutes === "number" &&
          typeof refreshExpiryDays === "number";

        if (hasExpiryMeta) {
          setAuthTokens(
            updatedAccessToken,
            updatedRefreshToken,
            accessExpiryMinutes,
            refreshExpiryDays
          );
        } else {
          setAuthTokens(updatedAccessToken, updatedRefreshToken);
        }
      }
    }
    return response;
  },
  async (error: AxiosError) => {
    if (error.response) {
      const { status, data } = error.response;
      if (status === 0) {
        console.error("[API ERROR] Network Error - Unable to reach the server");
        return Promise.reject(error);
      }

      if (status === 401) {
        console.error("[API ERROR] refresh token expired - redirecting to login");
        await clearAuthTokens();
        redirectToLogin();
        return Promise.reject(error);
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
