import { useQuery } from "@tanstack/react-query";

import { getHttpClient } from "@helpers/httpClient";

const resource = "/Converter/KeyHistory";

export const fetchConverterKeyHistory = async () => {
  const client = getHttpClient();
  const response = await client.get<UserUseKeyHistoryResponse[]>(resource);
  return response.data;
};

export const useConverterKeyHistoryQuery = (options?: { enabled?: boolean }) => {
  return useQuery<UserUseKeyHistoryResponse[]>({
    queryKey: ["converter", "key-history"],
    queryFn: fetchConverterKeyHistory,
    staleTime: 60 * 1000,
    enabled: options?.enabled ?? true,
  });
};
