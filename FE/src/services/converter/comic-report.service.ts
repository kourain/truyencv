import { useQuery } from "@tanstack/react-query";

import { getHttpClient } from "@helpers/httpClient";
import { ComicReportResponse } from "../../types/comic-report";
import { ReportStatus } from "@const/enum/report-status";

export type ConverterReportListParams = {
  offset?: number;
  limit?: number;
  status?: ReportStatus | null;
};

const resource = "/Converter/ComicReport";

export const fetchConverterReports = async (params: ConverterReportListParams = {}) => {
  const client = getHttpClient();
  const response = await client.get<ComicReportResponse[]>(resource, { params });
  return response.data;
};

export const fetchConverterReportById = async (id: string) => {
  const client = getHttpClient();
  const response = await client.get<ComicReportResponse>(`${resource}/${id}`);
  return response.data;
};

export const useConverterReportsQuery = (params: ConverterReportListParams = {}, options?: { enabled?: boolean }) => {
  return useQuery<ComicReportResponse[]>({
    queryKey: ["converter", "reports", params],
    queryFn: () => fetchConverterReports(params),
    staleTime: 60 * 1000,
    enabled: options?.enabled ?? true,
  });
};
