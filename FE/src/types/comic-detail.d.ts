interface ComicDetailCategory {
  id: string;
  name: string;
}

interface ComicDetailChapter {
  id: string;
  number: number;
  title: string;
  released_at: string;
}

interface ComicDetailAdvertisement {
  id: string;
  image_url: string;
  href: string;
  label: string;
  description?: string;
}

interface ComicDetailReview {
  id: string;
  user_display_name: string;
  rating: number;
  comment: string;
  created_at: string;
}

interface ComicDetailDiscussion {
  id: string;
  user_display_name: string;
  message: string;
  created_at: string;
}

interface ComicDetailRelatedComic {
  id: string;
  slug: string;
  title: string;
  cover_url?: string;
  latest_chapter?: number;
}
interface ComicSEO {
  title: string;
  description: string;
  keywords: string[];
  image: string;
}
interface Comic {
  id: string;
  slug: string;
  title: string;
  cover_url?: string;
  banner_url?: string;
  synopsis: string;
  author_name: string;
  rating_average: number;
  rating_count: number;
  bookmark_count: number;
  weekly_chapter_count: number;
  weekly_recommendations: number;
  user_last_read_chapter?: number;
  categories: ComicDetailCategory[];
};
interface ComicDetailResponse {
  comic: Comic;
  latest_chapters: ComicDetailChapter[];
  advertisements: {
    primary: ComicDetailAdvertisement;
    secondary: ComicDetailAdvertisement;
    tertiary: ComicDetailAdvertisement;
  };
  introduction: string;
  related_by_author: ComicDetailRelatedComic[];
  reviews: ComicDetailReview[];
  discussions: ComicDetailDiscussion[];
  highlights: string[];
}
