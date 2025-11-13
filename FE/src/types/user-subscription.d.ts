interface CreateUserSubscriptionRequest {
	user_id: string;
	subscription_id: string;
	start_at?: string | null;
	end_at?: string | null;
	is_active?: boolean;
	auto_renew?: boolean;
}

interface UpdateUserSubscriptionRequest {
	id: string;
	end_at?: string | null;
	is_active: boolean;
	auto_renew: boolean;
}

interface UserSubscriptionResponse {
	id: string;
	user_id: string;
	subscription_id: string;
	start_at: string;
	end_at: string | null;
	is_active: boolean;
	auto_renew: boolean;
	created_at: string;
	updated_at: string;
}
