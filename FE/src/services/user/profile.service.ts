import { useQuery } from "@tanstack/react-query";

import { getHttpClient } from "@helpers/httpClient";

const resource = "/auth/me";

export const fetchUserProfile = async (): Promise<UserProfileResponse> => {
  const client = getHttpClient();
  const response = await client.get<UserProfileResponse>(resource);
  return response.data;
};

export const useUserProfileQuery = (options?: { enabled?: boolean }) => {
  return useQuery<UserProfileResponse>({
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
