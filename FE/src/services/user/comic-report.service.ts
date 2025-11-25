import { getHttpClient } from "@helpers/httpClient";
import type { ComicReportResponse, CreateComicReportRequest } from "../../types/comic-report";

const resource = "/User/ComicReport";

export const createUserComicReport = async (payload: CreateComicReportRequest) => {
  const client = getHttpClient();
  const response = await client.post<ComicReportResponse>(resource, payload);
  return response.data;
};

export const getUserComicReports = async (offset: number = 0, limit: number = 20) => {
  const client = getHttpClient();
  const response = await client.get<ComicReportResponse[]>(resource, {
    params: { offset, limit }
  });
  return response.data;
};
