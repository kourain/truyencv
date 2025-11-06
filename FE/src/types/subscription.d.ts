interface CreateSubscriptionRequest {
	code: string;
	name: string;
	description?: string | null;
	price_coin: number;
	duration_day: number;
	is_active?: boolean;
}

interface UpdateSubscriptionRequest {
	id: string;
	code: string;
	name: string;
	description?: string | null;
	price_coin: number;
	duration_day: number;
	is_active: boolean;
}

interface SubscriptionResponse {
	id: string;
	code: string;
	name: string;
	description: string | null;
	price_coin: number;
	duration_day: number;
	is_active: boolean;
	created_at: string;
	updated_at: string;
}
