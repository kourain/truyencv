import { useQuery } from "@tanstack/react-query";

import { getHttpClient } from "@helpers/httpClient";

export type UserProfileResponse = {
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

export type ChangePasswordPayload = {
  current_password: string;
  new_password: string;
};

const resource = "/auth/me";

export const fetchUserProfile = async (): Promise<UserProfileResponse> => {
  const client = getHttpClient();
  const response = await client.get<UserProfileResponse>(resource);
  return response.data;
};

export const useUserProfileQuery = () => {
  return useQuery<UserProfileResponse>({
    queryKey: ["user-profile"],
    queryFn: fetchUserProfile,
    staleTime: 60 * 1000,
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
