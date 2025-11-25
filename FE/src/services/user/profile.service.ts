import { useQuery } from "@tanstack/react-query";

import { getHttpClient } from "@helpers/httpClient";
import { AxiosError } from "axios";

const resource = "/auth/me";

export const fetchUserProfile = async (): Promise<UserProfileResponse & AuthTokensResponse> => {
  const client = getHttpClient();
  try {
    const response = await client.get<UserProfileResponse>(resource);
    const accessToken = response.headers["x-access-token"] || null;
    const accessTokenExpiryMinutes = response.headers["x-access-token-expiry"] || null;
    const refreshToken = response.headers["x-refresh-token"] || null;
    const refreshTokenExpiryDays = response.headers["x-refresh-token-expiry"] || null;
    return {
      ...response.data,
      access_token: accessToken, access_token_minutes: accessTokenExpiryMinutes,
      refresh_token: refreshToken, refresh_token_days: refreshTokenExpiryDays
    };
  } catch (error) {
    if (error instanceof AxiosError) {
      if (error.response?.status === 401 || error.response?.status === 403) {
        return { id: "-1" } as UserProfileResponse & AuthTokensResponse;
      }
    }
    return {} as UserProfileResponse & AuthTokensResponse;
  }
};

export const useUserProfileQuery = (options?: { enabled?: boolean }) => {
  return useQuery<UserProfileResponse & AuthTokensResponse>({
    queryKey: ["user-profile"],
    queryFn: fetchUserProfile,
    staleTime: 60 * 1000,
    enabled: options?.enabled ?? true,
  });
};

export const verifyEmail = async (): Promise<UserProfileResponse> => {
  const client = getHttpClient();
  const response = await client.post<UserProfileResponse>("/user/profile/verify-email");
  return response.data;
};

export const changePassword = async (payload: ChangePasswordPayload): Promise<BaseResponse> => {
  const client = getHttpClient();
  const response = await client.post<BaseResponse>("/user/profile/change-password", payload);
  return response.data;
};

export const changeEmail = async (payload: ChangeEmailPayload): Promise<UserProfileResponse> => {
  const client = getHttpClient();
  const response = await client.post<UserProfileResponse>("/user/profile/change-email", payload);
  return response.data;
};

export const unlinkFirebaseAccount = async (): Promise<UserProfileResponse> => {
  const client = getHttpClient();
  const response = await client.post<UserProfileResponse>("/user/profile/unlink-firebase");
  return response.data;
};

export const fetchUserActiveSessions = async (): Promise<UserActiveSessionResponse[]> => {
  const client = getHttpClient();
  const response = await client.get<UserActiveSessionResponse[]>("/user/profile/refresh-tokens");
  return response.data.filter((session) => session.is_active);
};

export const revokeUserSession = async (sessionId: string): Promise<BaseResponse> => {
  const client = getHttpClient();
  const response = await client.delete<BaseResponse>(`/user/profile/refresh-tokens/${sessionId}`);
  return response.data;
};

export const revokeAllUserSessions = async (): Promise<BaseResponse> => {
  const client = getHttpClient();
  const response = await client.delete<BaseResponse>("/user/profile/refresh-tokens");
  return response.data;
};

export const fetchUserCoinHistory = async (): Promise<UserUseCoinHistoryResponse[]> => {
  const client = getHttpClient();
  const response = await client.get<UserUseCoinHistoryResponse[]>("/User/CoinHistory");
  return response.data;
};

export const fetchUserKeyHistory = async (): Promise<UserUseKeyHistoryResponse[]> => {
  const client = getHttpClient();
  const response = await client.get<UserUseKeyHistoryResponse[]>("/User/KeyHistory");
  return response.data;
};

export const fetchUserPaymentHistory = async (): Promise<PaymentHistoryResponse[]> => {
  const client = getHttpClient();
  const response = await client.get<PaymentHistoryResponse[]>("/User/PaymentHistory");
  return response.data;
};

export const fetchUserReadHistory = async (limit = 50): Promise<UserComicReadHistoryResponse[]> => {
  const client = getHttpClient();
  const response = await client.get<UserComicReadHistoryResponse[]>("/User/ReadHistory", {
    params: { limit },
  });
  return response.data;
};

export const fetchUserCommentHistory = async (): Promise<ComicCommentResponse[]> => {
  const client = getHttpClient();
  const response = await client.get<ComicCommentResponse[]>("/User/ComicComment/me");
  return response.data;
};

export const useUserCoinHistoryQuery = () => {
  return useQuery<UserUseCoinHistoryResponse[]>({
    queryKey: ["user-profile", "coin-history"],
    queryFn: fetchUserCoinHistory,
    staleTime: 60 * 1000,
  });
};

export const useUserKeyHistoryQuery = () => {
  return useQuery<UserUseKeyHistoryResponse[]>({
    queryKey: ["user-profile", "key-history"],
    queryFn: fetchUserKeyHistory,
    staleTime: 60 * 1000,
  });
};

export const useUserPaymentHistoryQuery = () => {
  return useQuery<PaymentHistoryResponse[]>({
    queryKey: ["user-profile", "payment-history"],
    queryFn: fetchUserPaymentHistory,
    staleTime: 60 * 1000,
  });
};

export const useUserReadHistoryQuery = (limit = 50) => {
  return useQuery<UserComicReadHistoryResponse[]>({
    queryKey: ["user-profile", "read-history", limit],
    queryFn: () => fetchUserReadHistory(limit),
    staleTime: 60 * 1000,
  });
};

export const useUserCommentHistoryQuery = () => {
  return useQuery<ComicCommentResponse[]>({
    queryKey: ["user-profile", "comment-history"],
    queryFn: fetchUserCommentHistory,
    staleTime: 60 * 1000,
  });
};

export const useUserActiveSessionsQuery = (options?: { enabled?: boolean }) => {
  return useQuery<UserActiveSessionResponse[]>({
    queryKey: ["user-profile", "active-sessions"],
    queryFn: fetchUserActiveSessions,
    staleTime: 30 * 1000,
    enabled: options?.enabled ?? true,
  });
};
