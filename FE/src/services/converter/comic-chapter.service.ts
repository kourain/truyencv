import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

import { getHttpClient } from "@helpers/httpClient";

const resource = "/Converter/ComicChapter";

export const fetchConverterChapters = async (comicId: string) => {
  const client = getHttpClient();
  const response = await client.get<ComicChapterResponse[]>(`${resource}/comic/${comicId}`);
  return response.data;
};

export const fetchConverterChapterById = async (id: string) => {
  const client = getHttpClient();
  const response = await client.get<ComicChapterResponse>(`${resource}/${id}`);
  return response.data;
};

export const createConverterChapter = async (payload: CreateComicChapterRequest) => {
  const client = getHttpClient();
  const response = await client.post<ComicChapterResponse>(resource, payload);
  return response.data;
};

export const updateConverterChapter = async (payload: UpdateComicChapterRequest) => {
  const client = getHttpClient();
  const response = await client.put<ComicChapterResponse>(`${resource}/${payload.id}`, payload);
  return response.data;
};

export const deleteConverterChapter = async (id: string) => {
  const client = getHttpClient();
  const response = await client.delete<BaseResponse>(`${resource}/${id}`);
  return response.data;
};

export const useConverterChaptersQuery = (comicId?: string, options?: { enabled?: boolean }) => {
  return useQuery<ComicChapterResponse[]>({
    queryKey: ["converter", "chapters", comicId],
    queryFn: () => fetchConverterChapters(comicId ?? ""),
    staleTime: 60 * 1000,
    enabled: Boolean(comicId) && (options?.enabled ?? true),
  });
};

export const useCreateConverterChapterMutation = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: createConverterChapter,
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({ queryKey: ["converter", "chapters", variables.comic_id] });
    },
  });
};

export const useUpdateConverterChapterMutation = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: updateConverterChapter,
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({ queryKey: ["converter", "chapters", variables.comic_id] });
    },
  });
};

export const useDeleteConverterChapterMutation = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: deleteConverterChapter,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["converter", "chapters"] });
    },
  });
};
