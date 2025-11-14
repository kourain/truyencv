import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

import { getHttpClient } from "@helpers/httpClient";

const resource = "/Converter/ComicComment";

export const fetchConverterComments = async (comicId: string) => {
  const client = getHttpClient();
  const response = await client.get<ComicCommentResponse[]>(`${resource}/comic/${comicId}`);
  return response.data;
};

export const fetchConverterCommentById = async (id: string) => {
  const client = getHttpClient();
  const response = await client.get<ComicCommentResponse>(`${resource}/${id}`);
  return response.data;
};

export const deleteConverterComment = async (id: string) => {
  const client = getHttpClient();
  const response = await client.delete<BaseResponse>(`${resource}/${id}`);
  return response.data;
};

export const useConverterCommentsQuery = (comicId?: string, options?: { enabled?: boolean }) => {
  return useQuery<ComicCommentResponse[]>({
    queryKey: ["converter", "comments", comicId],
    queryFn: () => fetchConverterComments(comicId ?? ""),
    staleTime: 60 * 1000,
    enabled: Boolean(comicId) && (options?.enabled ?? true),
  });
};

export const useDeleteConverterCommentMutation = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: deleteConverterComment,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["converter", "comments"] });
    },
  });
};
