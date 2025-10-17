import { useQuery } from "@tanstack/react-query";

import { getHttpClient } from "@helpers/httpClient";

const resource = (slug: string, chapterNumber: string | number) => `/user/comic/${slug}/chapters/${chapterNumber}`;

export const fetchUserComicChapter = async (
	slug: string,
	chapterNumber: string | number
): Promise<ComicChapterReadResponse> => {
	const client = getHttpClient();

	try {
		const response = await client.get<ComicChapterReadResponse>(resource(slug, chapterNumber));
		return response.data;
	} catch (error) {
		if (process.env.NODE_ENV !== "production") {
			console.warn("Sử dụng dữ liệu mock cho trang đọc chương", error);
		}

		return createMockChapter(slug, Number(chapterNumber));
	}
};

export const useUserComicChapterQuery = (slug: string, chapterNumber: string | number) => {
	return useQuery<ComicChapterReadResponse>({
		queryKey: ["user-comic-chapter", slug, chapterNumber],
		queryFn: () => fetchUserComicChapter(slug, chapterNumber),
		enabled: Boolean(slug) && Boolean(chapterNumber),
		staleTime: 60 * 1000,
	});
};

const createMockChapter = (slug: string, chapterNumber: number): ComicChapterReadResponse => {
	const chapterTitle = `Chương ${chapterNumber}: Khởi nguồn hành trình`;
	return {
		comic_id: "1001",
		comic_slug: slug,
		comic_title: slug
			.split("-")
			.map((segment) => segment.charAt(0).toUpperCase() + segment.slice(1))
			.join(" "),
		author_name: "Tác giả Ẩn Danh",
		chapter_id: String(100000 + chapterNumber),
		chapter_number: chapterNumber,
		chapter_title: chapterTitle,
		content: mockContent,
		updated_at: new Date().toISOString(),
		previous_chapter_number: chapterNumber > 1 ? chapterNumber - 1 : null,
		previous_chapter_id: chapterNumber > 1 ? String(100000 + chapterNumber - 1) : null,
		next_chapter_number: chapterNumber + 1,
		next_chapter_id: String(100000 + chapterNumber + 1),
		recommended_comic_title: "Truyền Thuyết Huyền Ảo",
		recommended_comic_slug: "truyen-thuyet-huyen-ao",
		monthly_recommendations: 128,
		month: new Date().getMonth() + 1,
		year: new Date().getFullYear(),
	};
};

const mockContent = `Sau khi vượt qua vô vàn thử thách, nhân vật chính cuối cùng cũng chạm tới cánh cổng cổ đại.
Ánh sáng chói lòa bao phủ khắp không gian, những ký tự cổ xưa bắt đầu thức tỉnh.

"Đây chính là sức mạnh mà ta đã tìm kiếm bấy lâu nay...", nhân vật chính khẽ thì thầm.

Nhưng ngay khoảnh khắc ấy, một bóng hình bí ẩn xuất hiện phía sau, mang theo luồng sát khí lạnh lẽo.
Cuộc chiến mới chỉ vừa bắt đầu, và vận mệnh thế giới đang nằm trong tay của họ.`;
