import { HistoryStatus } from "./history-status";

declare global {
	interface CreateUserUseCoinHistoryRequest {
		user_id: string;
		coin: number;
		status: HistoryStatus;
		reason?: string | null;
		reference_id?: string | null;
		reference_type?: string | null;
	}

	interface UserUseCoinHistoryResponse {
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
}

export {};
