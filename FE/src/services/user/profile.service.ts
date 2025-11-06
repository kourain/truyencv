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
