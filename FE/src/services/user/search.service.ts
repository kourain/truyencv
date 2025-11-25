import { useQuery } from "@tanstack/react-query";

import { getHttpClient } from "@helpers/httpClient";

export type SearchComicResult = {
  id: string;
  name: string;
  slug: string;
  cover_url?: string;
  author?: string;
  description?: string;
  main_category?: string;
  chap_count: number;
  rate: number;
  rate_count: number;
  match_score: number; // 0.0 - 1.0, similarity score
};

export type SearchComicResponse = {
  results: SearchComicResult[];
  total: number;
  page: number;
  page_size: number;
};

export type SearchComicParams = {
  keyword: string;
  page?: number;
  page_size?: number;
};

const resource = "/user/comic/search";

export const fetchComicSearch = async (params: SearchComicParams): Promise<SearchComicResponse> => {
  const client = getHttpClient();

  try {
    const response = await client.get<SearchComicResponse>(resource, { params });
    return response.data;
  } catch (error) {
    if (process.env.NODE_ENV !== "production") {
      console.warn("Sử dụng dữ liệu mock cho trang tìm kiếm", error);
    }

    return buildMockSearchData(params.keyword, params.page ?? 1, params.page_size ?? 12);
  }
};

export const useComicSearchQuery = (params: SearchComicParams) => {
  const { keyword, page = 1, page_size = 12 } = params;

  return useQuery({
    queryKey: ["user-search", keyword, page, page_size],
    queryFn: () => fetchComicSearch({ keyword, page, page_size }),
    enabled: Boolean(keyword.trim()),
    staleTime: 2 * 60 * 1000,
  });
};

const buildMockSearchData = (keyword: string, page: number, pageSize: number): SearchComicResponse => {
  const normalizedKeyword = keyword || "Truyện";
  const startIndex = (page - 1) * pageSize;

  const results = Array.from({ length: pageSize }).map((_, index) => {
    const comicIndex = startIndex + index + 1;
    return {
      id: `${900 + comicIndex}`,
      name: `${normalizedKeyword} hấp dẫn ${comicIndex}`,
      slug: `${normalizedKeyword.toLowerCase()}-hap-dan-${comicIndex}`,
      cover_url: `https://picsum.photos/seed/search-${comicIndex}/160/220`,
      author: `Tác giả ${comicIndex}`,
      description: "Câu chuyện kỳ ảo với nhịp độ nhanh, nhân vật cá tính và những biến chuyển bất ngờ.",
      main_category: index % 3 === 0 ? "Tiên Hiệp" : index % 3 === 1 ? "Huyền Huyễn" : "Kỳ Ảo",
      chap_count: 120 + index,
      rate: parseFloat((3.5 + (index % 5) * 0.3).toFixed(1)),
      rate_count: 100 + index * 10,
      match_score: parseFloat((0.95 - index * 0.05).toFixed(2)),
    } satisfies SearchComicResult;
  });

  return {
    results,
    total: 120,
    page,
    page_size: pageSize,
  } satisfies SearchComicResponse;
};
