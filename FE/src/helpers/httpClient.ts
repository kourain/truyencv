import axios, { AxiosError, AxiosInstance, AxiosRequestConfig, AxiosResponse } from "axios";
import { appEnv } from "@const/env";

const defaultConfig: AxiosRequestConfig = {
  baseURL: appEnv.backendUrl || undefined,
  headers: {
    "Content-Type": "application/json"
  },
  timeout: 15000
};

const httpClient: AxiosInstance = axios.create(defaultConfig);

httpClient.interceptors.request.use((config) => {
  const nextConfig = { ...config };

  if (nextConfig.baseURL === undefined && appEnv.backendUrl) {
    nextConfig.baseURL = appEnv.backendUrl;
  }

  if (nextConfig.url && !/^https?:\/\//i.test(nextConfig.url)) {
    nextConfig.url = nextConfig.url.startsWith("/") ? nextConfig.url : `/${nextConfig.url}`;
  }

  return nextConfig;
});

httpClient.interceptors.response.use(
  (response: AxiosResponse) => response,
  (error: AxiosError) => {
    if (error.response) {
      const { status, data } = error.response;
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
    return appEnv.cdnUrl;
  }

  if (/^https?:\/\//i.test(path)) {
    return path;
  }

  const trimmedPath = path.startsWith("/") ? path.slice(1) : path;
  const base = appEnv.cdnUrl.replace(/\/+$/, "");

  return `${base}/${trimmedPath}`;
};

export type ApiError = AxiosError<{ message?: string; errors?: Record<string, string[]> }>;
