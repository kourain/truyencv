import { UseMutationOptions, useMutation } from "@tanstack/react-query";

import { ApiError, getHttpClient } from "@helpers/httpClient";

const resource = "/User/ComicUnlock";

export const unlockComicChapter = async (payload: UnlockComicChapterRequest): Promise<UserComicUnlockHistoryResponse> => {
  const client = getHttpClient();
  const response = await client.post<UserComicUnlockHistoryResponse>(resource, payload);
  return response.data;
};

export const useUnlockComicChapterMutation = (
  options?: UseMutationOptions<UserComicUnlockHistoryResponse, ApiError, UnlockComicChapterRequest>
) => {
  return useMutation<UserComicUnlockHistoryResponse, ApiError, UnlockComicChapterRequest>({
    mutationFn: unlockComicChapter,
    ...options,
  });
};
