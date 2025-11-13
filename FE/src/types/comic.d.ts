import { ComicStatus } from "@const/enum/comic-status";

interface CreateComicRequest {
	name: string;
	description: string;
	author: string;
	embedded_from?: string | null;
	embedded_from_url?: string | null;
	cover_url?: string | null;
	main_category_id?: number; // Optional - defaults to 1001 if not provided
	category_ids?: number[]; // Optional - additional categories to add to comic
	status?: ComicStatus;
	// Note: slug, chap_count, rate, banner_url are not allowed - auto-generated/calculated by backend
}

interface UpdateComicRequest {
	id: string;
	name: string;
	description: string;
	author: string;
	embedded_from: string | null;
	embedded_from_url: string | null;
	cover_url: string | null;
	chap_count: number;
  main_category_id?: number; // Optional - defaults to 1001 if not provided
  category_ids?: number[]; // Optional - additional categories to add to comic
	rate: number;
	status: ComicStatus;
}

interface ComicResponse {
	id: string;
	name: string;
	description: string;
	slug: string;
	author: string;
	embedded_from: string | null;
	embedded_from_url: string | null;
	cover_url: string | null;
	chap_count: number;
	bookmark_count: number;
	rate: number;
	main_category: string; // Category name (e.g., "Tiên Hiệp")
	status: ComicStatus;
	created_at: string;
	updated_at: string;
}
