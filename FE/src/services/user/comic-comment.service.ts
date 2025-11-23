import { getHttpClient } from "@helpers/httpClient";

const resource = "/User/ComicComment";

export const createUserComicComment = async (payload: CreateComicCommentRequest) => {
  const client = getHttpClient();
  try {
    // backend expects user_id to be filled server-side from JWT, so we post without user_id
    const response = await client.post<ComicCommentResponse>(resource, payload);
    return response.data;
  } catch (error) {
    console.error("API Error in createUserComicComment:", error);
    throw error;
  }
};
