import { HistoryStatus } from "./history-status";

export interface CreateUserUseCoinHistoryRequest {
	user_id: string;
	coin: number;
	status: HistoryStatus;
	reason?: string | null;
	reference_id?: string | null;
	reference_type?: string | null;
}

export interface UserUseCoinHistoryResponse {
	id: string;
	user_id: string;
	coin: number;
	status: HistoryStatus;
	reason: string | null;
	reference_id: string | null;
	reference_type: string | null;
	created_at: string;
	updated_at: string;
}
