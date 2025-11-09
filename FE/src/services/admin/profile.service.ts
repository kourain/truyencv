import { useQuery } from "@tanstack/react-query";

import { getHttpClient } from "@helpers/httpClient";

const resource = "/auth/me";

export const fetchAdminProfile = async (): Promise<UserProfileResponse> => {
  const client = getHttpClient();
  const response = await client.get<UserProfileResponse>(resource);
  return response.data;
};

export const useAdminProfileQuery = (options?: { enabled?: boolean }) => {
  return useQuery<UserProfileResponse>({
    queryKey: ["admin-profile"],
    queryFn: fetchAdminProfile,
    staleTime: 60 * 1000,
    enabled: options?.enabled ?? true
  });
};
