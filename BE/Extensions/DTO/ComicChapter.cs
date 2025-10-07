using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;
using TruyenCV.Models;

namespace TruyenCV;

public static partial class Extensions
{
	// Convert CreateComicChapterRequest to ComicChapter entity
	public static ComicChapter ToEntity(this CreateComicChapterRequest request)
	{
		return new ComicChapter
		{
			comic_id = request.comic_id,
			chapter = request.chapter,
			content = request.content,
			created_at = DateTime.UtcNow,
			updated_at = DateTime.UtcNow
		};
	}

	// Convert ComicChapter entity to ComicChapterResponse
	public static ComicChapterResponse ToRespDTO(this ComicChapter chapter)
	{
		return new ComicChapterResponse
		{
			id = chapter.id,
			comic_id = chapter.comic_id,
			chapter = chapter.chapter,
			content = chapter.content,
			created_at = chapter.created_at,
			updated_at = chapter.updated_at
		};
	}

	// Update ComicChapter entity from UpdateComicChapterRequest
	public static void UpdateFromRequest(this ComicChapter chapter, UpdateComicChapterRequest request)
	{
		chapter.comic_id = request.comic_id;
		chapter.chapter = request.chapter;
		chapter.content = request.content;
		chapter.updated_at = DateTime.UtcNow;
	}
}
