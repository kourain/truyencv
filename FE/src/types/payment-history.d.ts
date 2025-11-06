interface CreatePaymentHistoryRequest {
	user_id: string;
	amount_coin: number;
	amount_money: number;
	payment_method?: string;
	reference_id?: string | null;
	note?: string | null;
}

interface PaymentHistoryResponse {
	id: string;
	user_id: string;
	amount_coin: number;
	amount_money: number;
	payment_method: string;
	reference_id: string | null;
	note: string | null;
	user_email?: string | null;
	user_name?: string | null;
	created_at: string;
	updated_at: string;
}

interface PaymentRevenuePointResponse {
	date: string;
	total_amount_coin: number;
	total_amount_money: number;
}
