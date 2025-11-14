import { useQuery } from "@tanstack/react-query";

import { getHttpClient } from "@helpers/httpClient";

const resource = "/converter/ComicCategory";

export const fetchConverterComicCategories = async () => {
  const client = getHttpClient();
  const response = await client.get<ComicCategoryResponse[]>(`${resource}/all`);
  return response.data;
};

export const useConverterComicCategoriesQuery = (enabled = true) =>
  useQuery<ComicCategoryResponse[]>({
    queryKey: ["converter", "comic-categories"],
    queryFn: fetchConverterComicCategories,
    staleTime: 60 * 1000,
    enabled,
  });
