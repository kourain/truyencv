import { getHttpClient } from "@helpers/httpClient";

const resource = (comicId: string | number) => `/User/Recommend/${comicId}`;

export const recommendComic = async (comicId: string | number): Promise<ComicRecommendResponse> => {
  const client = getHttpClient();
  const res = await client.post<ComicRecommendResponse>(resource(comicId));
  return res.data;
};
