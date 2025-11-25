import { getHttpClient } from "@helpers/httpClient";

const resource = "/User/Bookmark";

export interface CreateBookmarkRequest {
  comic_id: string;
}

export interface BookmarkStatusResponse {
  comic_id: number;
  is_bookmarked: boolean;
}

export const createBookmark = async (comicId: string) => {
  const client = getHttpClient();
  const response = await client.post<UserComicBookmarkResponse>(resource, {
    comic_id: comicId
  });
  return response.data;
};

export const removeBookmark = async (comicId: string | number) => {
  const client = getHttpClient();
  const response = await client.delete(`${resource}/${comicId}`);
  return response.data;
};

export const checkBookmarkStatus = async (comicId: string | number) => {
  const client = getHttpClient();
  const response = await client.get<BookmarkStatusResponse>(`${resource}/${comicId}/status`);
  return response.data;
};

export const getUserBookmarks = async () => {
  const client = getHttpClient();
  const response = await client.get<UserComicBookmarkResponse[]>(resource);
  return response.data;
};

export const getUserBookmarksWithDetails = async () => {
  const client = getHttpClient();
  const response = await client.get<UserBookmarkWithComicDetail[]>(`${resource}/with-details`);
  return response.data;
};
