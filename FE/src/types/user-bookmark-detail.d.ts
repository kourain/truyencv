interface UserBookmarkWithComicDetail {
  id: string;
  comic_id: string;
  comic_title: string;
  comic_slug: string;
  comic_cover_url: string;
  user_last_read_chapter?: number;
  latest_chapter_number: number;
  bookmarked_at: string;
}
