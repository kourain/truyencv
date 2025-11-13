import { getHttpClient } from "@helpers/httpClient";
import type { ReportStatus } from "@const/enum/report-status";
import type { ComicReportResponse } from "../../types/comic-report";

const resource = "/admin/ComicReport";

export type ComicReportListParams = {
	offset?: number;
	limit?: number;
	status?: ReportStatus | "";
};

export type UpdateComicReportStatusPayload = {
	id: string;
	status: ReportStatus;
};

export const fetchComicReports = async (params: ComicReportListParams = {}) => {
	const client = getHttpClient();
	const sanitizedParams = { ...params };

	if (sanitizedParams.status === "") {
		delete sanitizedParams.status;
	}

	const response = await client.get<ComicReportResponse[]>(resource, {
		params: sanitizedParams,
	});

	return response.data;
};

export const fetchComicReportById = async (id: string) => {
	const client = getHttpClient();
	const response = await client.get<ComicReportResponse>(`${resource}/${id}`);

	return response.data;
};

export const updateComicReportStatus = async (payload: UpdateComicReportStatusPayload) => {
	const client = getHttpClient();
	const response = await client.put<ComicReportResponse>(`${resource}/${payload.id}/status`, payload);

	return response.data;
};

export const banComicFromReport = async (id: string) => {
	const client = getHttpClient();
	const response = await client.post<ComicReportResponse>(`${resource}/${id}/ban-comic`);

	return response.data;
};

export const hideCommentFromReport = async (id: string) => {
	const client = getHttpClient();
	const response = await client.post<ComicReportResponse>(`${resource}/${id}/hide-comment`);

	return response.data;
};
