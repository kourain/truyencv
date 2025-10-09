import { useQuery } from "@tanstack/react-query";
import { fetchPing } from "@services/health.service";

export const pingQueryKey = ["ping"];

export const usePingQuery = () =>
  useQuery({
    queryKey: pingQueryKey,
    queryFn: fetchPing,
    staleTime: 30_000,
    retry: 1
  });
