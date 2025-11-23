import { useQuery } from "@tanstack/react-query";

import { ApiError, getHttpClient } from "@helpers/httpClient";

const resource = (slug: string) => `/user/comic/${slug}/chapters`;

export const fetchUserComicChaptersList = async (slug: string): Promise<ComicChaptersListResponse> => {
	const client = getHttpClient();

	try {
		const response = await client.get<ComicChaptersListResponse>(resource(slug));
		return response.data;
	} catch (error) {
		throw error as ApiError;
	}
};

export const useUserComicChaptersListQuery = (slug: string) => {
	return useQuery<ComicChaptersListResponse, ApiError>({
		queryKey: ["user-comic-chapters-list", slug],
		queryFn: () => fetchUserComicChaptersList(slug),
		enabled: Boolean(slug),
		staleTime: 5 * 60 * 1000,
	});
};
