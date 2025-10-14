import { HistoryStatus } from "./history-status";

export interface CreateUserUseKeyHistoryRequest {
	user_id: string;
	key: number;
	status: HistoryStatus;
	chapter_id?: string | null;
	note?: string | null;
}

export interface UserUseKeyHistoryResponse {
	id: string;
	user_id: string;
	key: number;
	status: HistoryStatus;
	chapter_id: string | null;
	note: string | null;
	created_at: string;
	updated_at: string;
}
