import axios, { AxiosError, AxiosInstance, AxiosRequestConfig, AxiosResponse } from "axios";
import { appEnv, isBrowser } from "@const/env";
import { clearAuthTokens, getAccessToken, getRefreshToken, getSVAccessToken, getSVRefreshToken, setAuthTokens } from "./authTokens";

const defaultConfig: AxiosRequestConfig = {
  baseURL: appEnv.BACKEND_URL || undefined,
  headers: {
    "Content-Type": "application/json",
    "Access-Control-Allow-Origin": "*",
    "Access-Control-Allow-Credentials": "true",
  },
  timeout: 15000,
  allowAbsoluteUrls: true
};

export const redirectToLogin = () => {
  const { pathname, search } = window.location;
  const normalizedPath = pathname.toLowerCase();
  const currentUrl = `${pathname}${search}`;
  if (window.location.hostname.startsWith("localhost")) {
    if (pathname.startsWith("/admin")) {
      window.location.href = "/admin/auth/login";
      return;
    } else if (pathname.startsWith("/user")) {
      window.location.href = "/user/auth/login";
      return;
    }
  }
  const isAdminScope = appEnv.FE_ADMIN.includes(window.location.hostname);
  const loginBase = isAdminScope ? "/admin/auth/login" : "/user/auth/login";
  const normalizedLoginBase = loginBase.toLowerCase();
  const shouldAppendRedirect = currentUrl && !normalizedPath.startsWith(normalizedLoginBase);
  const loginUrl = shouldAppendRedirect
    ? `${loginBase}?redirect=${encodeURIComponent(currentUrl)}`
    : loginBase;

  window.location.replace(loginUrl);
};

const httpClient: AxiosInstance = axios.create(defaultConfig);

httpClient.interceptors.request.use(async (config) => {
  const nextConfig = { ...config };

  if (nextConfig.baseURL === undefined && appEnv.BACKEND_URL) {
    nextConfig.baseURL = appEnv.BACKEND_URL;
  }
  let accessToken;
  if (isBrowser) {
    accessToken = getAccessToken();
    nextConfig.headers["X-Refresh-Token"] = getRefreshToken() || "";
  } else {
    accessToken = await getSVAccessToken();
    nextConfig.headers["X-Refresh-Token"] = await getSVRefreshToken();
  }
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

const updateAuthToken = (headers: AxiosResponse["headers"]): boolean => {
  const newAccessToken = headers["x-access-token"] || "";
  const newRefreshToken = headers["x-refresh-token"] || "";
  const newAccessTokenExpiry = parseInt(headers["x-access-token-expiry"] || "1", 10);
  const newRefreshTokenExpiry = parseInt(headers["x-refresh-token-expiry"] || "30", 10);
  if (newAccessToken.length > 0 && newRefreshToken.length > 0) {
    setAuthTokens(newAccessToken, newRefreshToken, newAccessTokenExpiry, newRefreshTokenExpiry);
    return true;
  }
  return false;
};

httpClient.interceptors.response.use(
  (response: AxiosResponse) => {
    if (isBrowser) {
      const responseData = response?.data as Partial<AuthTokensResponse> | undefined;
      if (updateAuthToken(response.headers) === false && response.config.url!.includes("/auth/login")) {
        const bodyRefreshToken = responseData?.refresh_token ?? "";
        const bodyAccessToken = responseData?.access_token ?? "";
        const accessExpiryMinutes = responseData?.access_token_minutes;
        const refreshExpiryDays = responseData?.refresh_token_days;
        const hasExpiryMeta =
          typeof accessExpiryMinutes === "number" &&
          typeof refreshExpiryDays === "number";

        if (hasExpiryMeta) {
          setAuthTokens(
            bodyAccessToken,
            bodyRefreshToken,
            accessExpiryMinutes,
            refreshExpiryDays
          );
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
      if (isBrowser)
        if (status === 401) {
          console.error("[API ERROR] 401 url:", error.request.responseURL);
          if (error.request.responseURL.includes("/auth/logout") === false)
            await clearAuthTokens();
          if (window.location.pathname.includes("/auth") === false) {
            console.error("[API ERROR] refresh token expired - redirecting to login");
            redirectToLogin();
          }
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
