using TruyenCV.DTOs.Response;
using TruyenCV.Repositories;

namespace TruyenCV;

public static partial class Extensions
{
	private static readonly Dictionary<long, string> categoryDict = new()
	{
		{ 1001, "Tiên Hiệp" },
		{ 1002, "Huyền Huyễn" },
		{ 1003, "Khoa Huyễn" },
		{ 1004, "Võng Du" },
		{ 1005, "Đô Thị" },
		{ 1006, "Đồng Nhân" },
		{ 1007, "Dã Sử" },
		{ 1008, "Cạnh Kỹ" },
		{ 1009, "Huyền Nghi" },
		{ 1010, "Kiếm Hiệp" },
		{ 1011, "Kỳ Ảo" },
		{ 1012, "Light Novel" }
	};

	/// <summary>
	/// Convert ComicSearchResult to SearchComicResponse DTO
	/// </summary>
	public static SearchComicResponse ToSearchRespDTO(this ComicSearchResult result)
	{
		var comic = result.Comic;
		var mainCategory = categoryDict.TryGetValue(comic.main_category_id, out var cat)
			? cat
			: "Unknown";

		return new SearchComicResponse
		{
			id = comic._id,
			name = comic.name,
			slug = comic.slug,
			cover_url = comic.cover_url,
			author = comic.author,
			description = comic.description,
			main_category = mainCategory,
			chap_count = comic.chapter_count,
			rate = comic.rate,
			rate_count = comic.rate_count,
			match_score = result.Score
		};
	}
}
