import { getHttpClient } from "@helpers/httpClient";

const resource = "/admin/PaymentHistory";

export type PaymentHistoryListParams = {
	offset?: number;
	limit?: number;
	keyword?: string;
};

export const fetchPaymentHistories = async (params: PaymentHistoryListParams = {}) => {
	const client = getHttpClient();
	const response = await client.get<PaymentHistoryResponse[]>(resource, { params });

	return response.data;
};

export const fetchPaymentHistoriesByUser = async (userId: string) => {
	const client = getHttpClient();
	const response = await client.get<PaymentHistoryResponse[]>(`${resource}/by-user/${userId}`);

	return response.data;
};

export const createPaymentHistory = async (payload: CreatePaymentHistoryRequest) => {
	const client = getHttpClient();
	const response = await client.post<PaymentHistoryResponse>(resource, payload);

	return response.data;
};

export const fetchPaymentRevenueSummary = async (days = 60) => {
	const client = getHttpClient();
	const response = await client.get<PaymentRevenuePointResponse[]>(`${resource}/revenue/summary`, {
		params: { days },
	});

	return response.data;
};
