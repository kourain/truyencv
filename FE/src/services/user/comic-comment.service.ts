import { getHttpClient } from "@helpers/httpClient";

const resource = "/user/ComicComment";

export const createUserComicComment = async (payload: Omit<CreateComicCommentRequest, "user_id">) => {
  const client = getHttpClient();
  // backend expects user_id to be filled server-side from JWT, so we post without user_id
  const response = await client.post<ComicCommentResponse>(resource, payload);
  return response.data;
};
