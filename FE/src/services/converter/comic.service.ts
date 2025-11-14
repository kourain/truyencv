import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

import { getHttpClient } from "@helpers/httpClient";
import { ComicResponse, CreateComicRequest, UpdateComicRequest } from "../../types/comic";

export type ConverterComicListParams = {
  offset?: number;
  limit?: number;
};

const resource = "/Converter/Comic";

export const fetchConverterComics = async (params: ConverterComicListParams = {}) => {
  const client = getHttpClient();
  const response = await client.get<ComicResponse[]>(resource, { params });
  return response.data;
};

export const fetchConverterComicById = async (id: string) => {
  const client = getHttpClient();
  const response = await client.get<ComicResponse>(`${resource}/${id}`);
  return response.data;
};

export const fetchConverterComicCategoriesOfComic = async (id: string) => {
  const client = getHttpClient();
  const response = await client.get<ComicCategoryResponse[]>(`${resource}/${id}/categories`);
  return response.data;
};

export const createConverterComic = async (payload: CreateComicRequest) => {
  const client = getHttpClient();
  const response = await client.post<ComicResponse>(resource, payload);
  return response.data;
};

export const updateConverterComic = async (payload: UpdateComicRequest) => {
  const client = getHttpClient();
  const response = await client.put<ComicResponse>(`${resource}/${payload.id}`, payload);
  return response.data;
};

export const deleteConverterComic = async (id: string) => {
  const client = getHttpClient();
  const response = await client.delete<BaseResponse>(`${resource}/${id}`);
  return response.data;
};

export const useConverterComicsQuery = (params: ConverterComicListParams = {}, options?: { enabled?: boolean }) => {
  return useQuery<ComicResponse[]>({
    queryKey: ["converter", "comics", params],
    queryFn: () => fetchConverterComics(params),
    staleTime: 60 * 1000,
    enabled: options?.enabled ?? true,
  });
};

export const useCreateConverterComicMutation = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: createConverterComic,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["converter", "comics"] });
    },
  });
};

export const useUpdateConverterComicMutation = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: updateConverterComic,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["converter", "comics"] });
    },
  });
};

export const useDeleteConverterComicMutation = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: deleteConverterComic,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["converter", "comics"] });
    },
  });
};
