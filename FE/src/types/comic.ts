import { ComicStatus } from "./comic-status";

export interface CreateComicRequest {
	name: string;
	description: string;
	slug: string;
	author: string;
	embedded_from?: string | null;
	embedded_from_url?: string | null;
	chap_count?: number;
	rate?: number;
	status?: ComicStatus;
}

export interface UpdateComicRequest {
	id: number;
	name: string;
	description: string;
	slug: string;
	author: string;
	embedded_from: string | null;
	embedded_from_url: string | null;
	chap_count: number;
	rate: number;
	status: ComicStatus;
}

export interface ComicResponse {
	id: number;
	name: string;
	description: string;
	slug: string;
	author: string;
	embedded_from: string | null;
	embedded_from_url: string | null;
	chap_count: number;
	rate: number;
	status: ComicStatus;
	created_at: string;
	updated_at: string;
}
