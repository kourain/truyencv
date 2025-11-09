import { useQuery } from "@tanstack/react-query";

import { getHttpClient } from "@helpers/httpClient";

const resource = (slug: string) => `/user/comic/${slug}`;

export const fetchUserComicSEO = async (slug: string): Promise<ComicSEO> => {
  const client = getHttpClient();

  try {
    const response = await client.get<ComicSEO>("/user/comic/seo/" + slug);
    return response.data;
  } catch {
    return {
      title: "Truyện hot trong tuần",
      description:
        "Một hành trình tu luyện đầy thử thách giữa thế giới huyền huyễn, nơi nhân vật chính phải vượt qua muôn vàn khó khăn để bảo vệ những người thân yêu.",
      keywords: ["truyện", "huyền huyễn", "tu luyện"],
      image: "https://picsum.photos/seed/comic-cover/320/480",
    };
  }
};
export const fetchUserComicDetail = async (slug: string): Promise<ComicDetailResponse> => {
  const client = getHttpClient();

  try {
    const response = await client.get<ComicDetailResponse>(resource(slug));
    return response.data;
  } catch (error) {
    if (process.env.NODE_ENV !== "production") {
      console.warn("Sử dụng dữ liệu mock cho trang chi tiết truyện", error);
    }

    return createMockComicDetail(slug);
  }
};

export const useUserComicDetailQuery = (slug: string) => {
  return useQuery<ComicDetailResponse>({
    queryKey: ["user-comic-detail", slug],
    queryFn: () => fetchUserComicDetail(slug),
    enabled: Boolean(slug),
    staleTime: 5 * 60 * 1000,
  });
};

const createMockComicDetail = (slug: string): ComicDetailResponse => {
  const baseTitle = slug
    .split("-")
    .map((segment) => segment.charAt(0).toUpperCase() + segment.slice(1))
    .join(" ");

  const now = new Date();
  const recentChapters: ComicDetailChapter[] = Array.from({ length: 4 }).map((_, index) => ({
    id: `${100 + index}`,
    number: 120 - index,
    title: `Chương ${120 - index}: Bí mật được hé lộ`,
    released_at: new Date(now.getTime() - index * 6 * 60 * 60 * 1000).toISOString(),
  }));

  return {
    comic: {
      id: "1001",
      slug,
      title: baseTitle || "Truyện hot trong tuần",
      cover_url: "https://picsum.photos/seed/comic-cover/320/480",
      banner_url: "https://picsum.photos/seed/comic-banner/1200/400",
      synopsis:
        "Một hành trình tu luyện đầy thử thách giữa thế giới huyền huyễn, nơi nhân vật chính phải vượt qua muôn vàn khó khăn để bảo vệ những người thân yêu.",
      author_name: "Tác Giả Ẩn Danh",
      rate: 4.6,
      rate_count: 1280,
      bookmark_count: 45230,
      weekly_chapter_count: 6,
      weekly_recommendations: 320,
      user_last_read_chapter: 110,
      categories: [
        { id: "1", name: "Huyền Huyễn" },
        { id: "2", name: "Tu Tiên" },
        { id: "3", name: "Phiêu Lưu" },
      ],
    },
    latest_chapters: recentChapters,
    advertisements: {
      primary: {
        id: "ad-primary",
        image_url: "https://picsum.photos/seed/ad-primary/1200/280",
        href: "https://truyencv.vn/khuyen-doc",
        label: "Tuyển tập truyện hot",
        description: "Khám phá ngay bộ truyện đang được đề cử nhiều nhất tuần này",
      },
      secondary: {
        id: "ad-secondary",
        image_url: "https://picsum.photos/seed/ad-secondary/1200/200",
        href: "https://truyencv.vn/su-kien",
        label: "Sự kiện độc giả tháng 10",
        description: "Đăng nhập mỗi ngày để nhận thêm chìa khóa và quà tặng hấp dẫn",
      },
      tertiary: {
        id: "ad-tertiary",
        image_url: "https://picsum.photos/seed/ad-tertiary/1200/220",
        href: "https://truyencv.vn/khuyen-mai",
        label: "Gói thuê bao ưu đãi",
        description: "Tiết kiệm đến 40% khi đăng ký gói đọc truyện trong 3 tháng",
      },
    },
    introduction:
      "Ngày xưa, trong thế giới nơi linh khí tràn đầy, một thiếu niên mang trong mình số mệnh truyền thừa cổ xưa đã bước vào con đường tu luyện bất tận. Với ý chí kiên định và những người đồng đội nghĩa tình, cậu dần khám phá ra bí mật về nguồn gốc của mình và đối đầu với các thế lực hắc ám trong bóng tối.",
    related_by_author: Array.from({ length: 6 }).map((_, index) => ({
      id: `${2000 + index}`,
      slug: `tac-gia-an-danh-tac-pham-${index + 1}`,
      title: `Tác phẩm nổi bật ${index + 1}`,
      cover_url: `https://picsum.photos/seed/related-${index + 1}/160/220`,
      latest_chapter: 80 + index * 5,
    })),
    reviews: Array.from({ length: 4 }).map((_, index) => ({
      id: `${3000 + index}`,
      user_display_name: `Độc giả ${index + 1}`,
      rating: 4 - (index % 2) * 0.5,
      comment: "Cốt truyện hấp dẫn, nhân vật phát triển tốt và nhịp độ truyện vừa phải.",
      created_at: new Date(now.getTime() - (index + 1) * 3600 * 1000).toISOString(),
    })),
    discussions: Array.from({ length: 5 }).map((_, index) => ({
      id: `${4000 + index}`,
      user_display_name: `Thành viên ${index + 2}`,
      message: "Có ai đoán được thân phận thật sự của nhân vật phản diện không?",
      created_at: new Date(now.getTime() - (index + 1) * 5400 * 1000).toISOString(),
    })),
    highlights: [
      "Chương mới cập nhật mỗi ngày",
      "Miễn phí 3 chương đầu tiên cho thành viên",
      "Sự kiện đua TOP fan cứng với quà tặng đặc biệt",
    ],
  };
};
