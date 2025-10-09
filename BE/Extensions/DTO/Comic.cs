using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;
using TruyenCV.Models;

namespace TruyenCV;

public static partial class Extensions
{
	// Convert CreateComicRequest to Comic entity
	public static Comic ToEntity(this CreateComicRequest request)
	{
		return new Comic
		{
			name = request.name,
			description = request.description,
			slug = request.slug,
			author = request.author,
			embedded_from = request.embedded_from,
			embedded_from_url = request.embedded_from_url,
			chapter_count = request.chap_count,
			rate = request.rate,
			status = request.status,
			created_at = DateTime.UtcNow,
			updated_at = DateTime.UtcNow
		};
	}

	// Convert Comic entity to ComicResponse
	public static ComicResponse ToRespDTO(this Comic comic)
	{
		return new ComicResponse
		{
			id = comic.id,
			name = comic.name,
			description = comic.description,
			slug = comic.slug,
			author = comic.author,
			embedded_from = comic.embedded_from,
			embedded_from_url = comic.embedded_from_url,
			chap_count = comic.chapter_count,
			rate = comic.rate,
			status = comic.status,
			created_at = comic.created_at,
			updated_at = comic.updated_at
		};
	}

	// Update Comic entity from UpdateComicRequest
	public static void UpdateFromRequest(this Comic comic, UpdateComicRequest request)
	{
		comic.name = request.name;
		comic.description = request.description;
		comic.slug = request.slug;
		comic.author = request.author;
		comic.embedded_from = request.embedded_from;
		comic.embedded_from_url = request.embedded_from_url;
		comic.chapter_count = request.chap_count;
		comic.rate = request.rate;
		comic.status = request.status;
		comic.updated_at = DateTime.UtcNow;
	}
}
