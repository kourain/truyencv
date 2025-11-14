import { useQuery } from "@tanstack/react-query";

import { getHttpClient } from "@helpers/httpClient";

const resource = "/Converter/CoinHistory";

export const fetchConverterCoinHistory = async () => {
  const client = getHttpClient();
  const response = await client.get<UserUseCoinHistoryResponse[]>(resource);
  return response.data;
};

export const useConverterCoinHistoryQuery = (options?: { enabled?: boolean }) => {
  return useQuery<UserUseCoinHistoryResponse[]>({
    queryKey: ["converter", "coin-history"],
    queryFn: fetchConverterCoinHistory,
    staleTime: 60 * 1000,
    enabled: options?.enabled ?? true,
  });
};
