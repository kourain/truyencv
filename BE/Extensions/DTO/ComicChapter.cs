using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;
using TruyenCV.Models;

namespace TruyenCV;

public static partial class Extensions
{
	// Convert CreateComicChapterRequest to ComicChapter entity
	public static ComicChapter ToEntity(this CreateComicChapterRequest request)
	{
		return new ComicChapter
		{
			comic_id = request.comic_id.ToSnowflakeId(nameof(request.comic_id)),
			chapter = request.chapter,
			content = request.content,
			key_require = request.key_require ?? 1,
			key_require_until = request.key_require_until
		};
	}

	// Convert ComicChapter entity to ComicChapterResponse
	public static ComicChapterResponse ToRespDTO(this ComicChapter chapter)
	{
		return new ComicChapterResponse
		{
			id = chapter._id,
			comic_id = chapter.comic_id.ToString(),
			chapter = chapter.chapter,
			content = chapter.content,
			key_require = chapter.key_require,
			key_require_until = chapter.key_require_until,
			created_at = chapter.created_at,
			updated_at = chapter.updated_at
		};
	}

	// Update ComicChapter entity from UpdateComicChapterRequest
	public static void UpdateFromRequest(this ComicChapter chapter, UpdateComicChapterRequest request)
	{
		chapter.comic_id = request.comic_id.ToSnowflakeId(nameof(request.comic_id));
		chapter.chapter = request.chapter;
		chapter.content = request.content;
		if (request.key_require.HasValue)
		{
			chapter.key_require = request.key_require.Value;
		}
		if (request.key_require_until != chapter.key_require_until)
		{
			chapter.key_require_until = request.key_require_until;
		}
	}
}
