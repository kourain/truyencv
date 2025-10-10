import { getHttpClient } from "@helpers/httpClient";
import type { AdminDashboardOverview } from "../../types/admin-dashboard";

const resource = "/admin/Dashboard";

export const fetchAdminDashboardOverview = async () => {
	const client = getHttpClient();
	const response = await client.get<AdminDashboardOverview>(`${resource}/overview`);

	return response.data;
};
