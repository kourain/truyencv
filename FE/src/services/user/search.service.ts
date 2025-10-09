import { useQuery } from "@tanstack/react-query";

import { getHttpClient } from "@helpers/httpClient";

export type SearchComicResult = {
  comic_id: number;
  comic_title: string;
  cover_url?: string;
  author_name?: string;
  latest_chapter?: number;
  total_chapters?: number;
  short_description?: string;
  genres?: string[];
  updated_at?: string;
  status_label?: string;
  rating?: number;
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
      comic_id: 900 + comicIndex,
      comic_title: `${normalizedKeyword} hấp dẫn ${comicIndex}`,
      cover_url: `https://picsum.photos/seed/search-${comicIndex}/160/220`,
      author_name: `Tác giả ${comicIndex}`,
      latest_chapter: 50 + index,
      total_chapters: 120 + index,
      short_description: "Câu chuyện kỳ ảo với nhịp độ nhanh, nhân vật cá tính và những biến chuyển bất ngờ.",
      genres: ["Huyền huyễn", "Hành động", index % 2 === 0 ? "Phiêu lưu" : "Kỳ ảo"],
      updated_at: new Date(Date.now() - index * 3600 * 1000).toISOString(),
      status_label: index % 3 === 0 ? "Đang ra" : "Hoàn thành",
      rating: 3.5 + (index % 5) * 0.3,
    } satisfies SearchComicResult;
  });

  return {
    results,
    total: 120,
    page,
    page_size: pageSize,
  } satisfies SearchComicResponse;
};
