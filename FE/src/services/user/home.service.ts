import { useQuery } from "@tanstack/react-query";

import { getHttpClient } from "@helpers/httpClient";


export type UserHomeResponse = {
  history: UserHistoryItem[];
  editor_picks: HighlightedComic[];
  top_recommended: RankingComic[];
  top_weekly_reads: RankingComic[];
  latest_updates: ComicUpdate[];
  recently_completed: CompletedComic[];
  latest_reviews: ReviewSummary[];
};

const resource = "/user/home";

export const fetchUserHome = async (): Promise<UserHomeResponse> => {
  const client = getHttpClient();

  try {
    const response = await client.get<UserHomeResponse>(resource);
    return response.data;
  } catch (error) {
    if (process.env.NODE_ENV !== "production") {
      console.warn("Sử dụng dữ liệu mock cho trang chủ người dùng", error);
    }

    return mockUserHomeData;
  }
};

export const useUserHomeQuery = () => {
  return useQuery<UserHomeResponse>({
    queryKey: ["user-home"],
    queryFn: fetchUserHome,
    staleTime: 5 * 60 * 1000,
  });
};

const mockUserHomeData: UserHomeResponse = {
  history: [
    {
      comic_id: 101,
      comic_title: "Đại Chúa Tể",
      comic_slug: "dai-chua-te",
      last_read_chapter: 120,
      total_chapters: 320,
      last_read_at: new Date().toISOString(),
      cover_url: "https://picsum.photos/seed/history-1/120/160",
    },
    {
      comic_id: 102,
      comic_title: "Long Huyết Chiến Thần",
      comic_slug: "long-huyet-chien-than",
      last_read_chapter: 88,
      total_chapters: 180,
      last_read_at: new Date().toISOString(),
      cover_url: "https://picsum.photos/seed/history-2/120/160",
    },
    {
      comic_id: 103,
      comic_title: "Tuyệt Thế Võ Thần",
      comic_slug: "tuyet-the-vo-than",
      last_read_chapter: 45,
      total_chapters: 200,
      last_read_at: new Date().toISOString(),
      cover_url: "https://picsum.photos/seed/history-3/120/160",
    },
    {
      comic_id: 104,
      comic_title: "Đấu Phá Thương Khung",
      comic_slug: "dau-pha-thuong-khung",
      last_read_chapter: 310,
      total_chapters: 400,
      last_read_at: new Date().toISOString(),
      cover_url: "https://picsum.photos/seed/history-4/120/160",
    },
    {
      comic_id: 105,
      comic_title: "Huyền Huyễn Chí Tôn",
      comic_slug: "huyen-huyen-chi-ton",
      last_read_chapter: 22,
      total_chapters: 120,
      last_read_at: new Date().toISOString(),
      cover_url: "https://picsum.photos/seed/history-5/120/160",
    },
  ],
  editor_picks: [
    {
      comic_id: 200,
      comic_title: "Thiên Hạ Vô Song",
      comic_slug: "thien-ha-vo-song",
      short_description: "Biên tập viên tuyển chọn với cốt truyện gay cấn, nhịp độ nhanh.",
      cover_url: "https://picsum.photos/seed/editor-1/160/200",
      average_rating: 4.7,
      latest_chapter: 210,
    },
    {
      comic_id: 201,
      comic_title: "Nhất Niệm Vĩnh Hằng",
      comic_slug: "nhat-niem-vinh-hang",
      short_description: "Hành trình tu tiên đầy cảm xúc và nhân văn.",
      cover_url: "https://picsum.photos/seed/editor-2/160/200",
      average_rating: 4.9,
      latest_chapter: 150,
    },
    {
      comic_id: 202,
      comic_title: "Đỉnh Cấp Thánh Chủ",
      comic_slug: "dinh-cap-thanh-chu",
      short_description: "Thế giới rộng lớn với nhiều tuyến nhân vật phức tạp.",
      cover_url: "https://picsum.photos/seed/editor-3/160/200",
      average_rating: 4.6,
      latest_chapter: 320,
    },
  ],
  top_recommended: Array.from({ length: 5 }).map((_, index) => ({
    comic_id: 300 + index,
    comic_title: `Truyện đề cử nóng ${index + 1}`,
    comic_slug: `truyen-de-cu-nong-${index + 1}`,
    cover_url: `https://picsum.photos/seed/recommended-${index + 1}/120/160`,
    total_views: 150000 + index * 5000,
    recommendation_score: 95 - index * 3,
  })),
  top_weekly_reads: Array.from({ length: 5 }).map((_, index) => ({
    comic_id: 400 + index,
    comic_title: `Đọc nhiều tuần này ${index + 1}`,
    comic_slug: `doc-nhieu-tuan-nay-${index + 1}`,
    cover_url: `https://picsum.photos/seed/weekly-${index + 1}/120/160`,
    total_views: 300000 + index * 8000,
    weekly_views: 50000 - index * 4000,
  })),
  latest_updates: Array.from({ length: 10 }).map((_, index) => ({
    comic_id: 500 + index,
    comic_title: `Truyện cập nhật ${index + 1}`,
    comic_slug: `truyen-cap-nhat-${index + 1}`,
    chapter_title: `Chương ${index + 50}: Diễn biến mới`,
    chapter_number: index + 50,
    updated_at: new Date(Date.now() - index * 3600 * 1000).toISOString(),
  })),
  recently_completed: Array.from({ length: 10 }).map((_, index) => ({
    comic_id: 600 + index,
    comic_title: `Truyện hoàn thành ${index + 1}`,
    comic_slug: `truyen-hoan-thanh-${index + 1}`,
    total_chapters: 100 + index * 10,
    cover_url: `https://picsum.photos/seed/completed-${index + 1}/140/180`,
    completed_at: new Date(Date.now() - index * 24 * 3600 * 1000).toISOString(),
  })),
  latest_reviews: Array.from({ length: 4 }).map((_, index) => ({
    review_id: 700 + index,
    comic_id: 500 + index,
    comic_title: `Truyện cập nhật ${index + 1}`,
    comic_slug: `truyen-cap-nhat-${index + 1}`,
    user_display_name: `Anh Hùng ${index + 1}`,
    rating: 4 - (index % 2) * 0.5,
    liked_count: 10 + index * 3,
    created_at: new Date(Date.now() - index * 7200 * 1000).toISOString(),
    content: "Tình tiết hấp dẫn, nhân vật phát triển rõ nét.",
  })),
};
