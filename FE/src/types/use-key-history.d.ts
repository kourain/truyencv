import { HistoryStatus } from "../const/enum/history-status";

declare global {
	interface CreateUserUseKeyHistoryRequest {
		user_id: string;
		key: number;
		status: HistoryStatus;
		chapter_id?: string | null;
		note?: string | null;
	}

	interface UserUseKeyHistoryResponse {
		id: string;
		user_id: string;
		key: number;
		status: HistoryStatus;
		chapter_id: string | null;
		comic_name?: string | null;
		chapter_number?: number | null;
		note: string | null;
		created_at: string;
		updated_at: string;
	}
}

export {};
