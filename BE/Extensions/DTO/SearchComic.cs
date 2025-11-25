using TruyenCV.DTOs.Response;
using TruyenCV.Models;

namespace TruyenCV;

public static partial class Extensions
{
	/// <summary>
	/// Convert Comic to SearchComicResponse DTO
	/// </summary>
	public static SearchComicResponse ToSearchRespDTO(this Comic comic)
	{
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
			rate_count = comic.rate_count
		};
	}
}
