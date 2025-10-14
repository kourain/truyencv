import { HistoryStatus } from "./history-status";

export interface CreateUserCoinHistoryRequest {
	user_id: string;
	coin: number;
	status: HistoryStatus;
	reason?: string | null;
	reference_id?: string | null;
	reference_type?: string | null;
}

export interface UserCoinHistoryResponse {
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
