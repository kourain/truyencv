import { useQuery, useQueryClient } from "@tanstack/react-query";
import { getHttpClient } from "@helpers/httpClient";

const resource = (slug: string) => `/user/comic/${slug}/embedded`;

export const fetchComicsEmbeddedBySlug = async (slug: string) => {
  const client = getHttpClient();
  const response = await client.get<ComicDetailRelatedComic[]>(resource(slug));
  return response.data;
};

export const useComicsEmbeddedBySlugQuery = (slug: string | undefined) => {
  return useQuery({
    queryKey: ["user-comic-embedded", slug],
    queryFn: () => fetchComicsEmbeddedBySlug(slug ?? ""),
    enabled: Boolean(slug),
    staleTime: 5 * 60 * 1000,
  });
};
