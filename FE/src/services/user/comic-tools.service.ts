import { getHttpClient } from "@helpers/httpClient";

const chapterResource = (slug: string, chapterNumber: string | number) => `/user/comic/${slug}/chapters/${chapterNumber}`;

type ChapterIdentity = {
  slug: string;
  chapterNumber: number | string;
};

type ConvertChapterToTvPayload = ChapterIdentity & ConvertChapterToTvRequest;

type ChapterTtsPayload = ChapterIdentity & ChapterTtsRequestPayload;

export const convertChapterToTv = async ({ slug, chapterNumber, content }: ConvertChapterToTvPayload) => {
  const client = getHttpClient();
  const response = await client.post<ConvertChapterToTvResponse>(`${chapterResource(slug, chapterNumber)}/convert-tv`, {
    content,
  });
  return response.data;
};

export const requestChapterTts = async ({ slug, chapterNumber, content, reference_audio }: ChapterTtsPayload) => {
  const client = getHttpClient();
  const response = await client.post(`${chapterResource(slug, chapterNumber)}/tts`, {
    content,
    reference_audio,
  }, {
    responseType: "blob",
  });
  return response.data as Blob;
};

export const fetchTtsVoices = async (): Promise<string[]> => {
  const client = getHttpClient();
  const response = await client.get<string[]>("/user/comic/tts/voices");
  return response.data;
};
