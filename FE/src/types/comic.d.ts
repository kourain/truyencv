import { ComicStatus } from "@const/enum/comic-status";

interface CreateComicRequest {
	name: string;
	description: string;
	author: string;
	embedded_from?: string | null;
	embedded_from_url?: string | null;
	cover_url?: string | null;
	chap_count?: number;
	rate?: number;
	status?: ComicStatus;
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
	status: ComicStatus;
	created_at: string;
	updated_at: string;
}
