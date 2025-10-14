using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;
using TruyenCV.Models;

namespace TruyenCV;

public static partial class Extensions
{
	// Convert CreateComicCommentRequest to ComicComment entity
	public static ComicComment ToEntity(this CreateComicCommentRequest request)
	{
		return new ComicComment
		{
			comic_id = request.comic_id.ToSnowflakeId(nameof(request.comic_id)),
			comic_chapter_id = request.comic_chapter_id.ToNullableSnowflakeId(nameof(request.comic_chapter_id)),
			user_id = request.user_id.ToSnowflakeId(nameof(request.user_id)),
			comment = request.comment,
			like = request.like,
			reply_id = request.reply_id.ToNullableSnowflakeId(nameof(request.reply_id)),
			is_rate = request.is_rate,
			rate_star = request.rate_star,
			created_at = DateTime.UtcNow,
			updated_at = DateTime.UtcNow
		};
	}

	// Convert ComicComment entity to ComicCommentResponse
	public static ComicCommentResponse ToRespDTO(this ComicComment commentEntity)
	{
		return new ComicCommentResponse
		{
			id = commentEntity._id,
			comic_id = commentEntity.comic_id.ToString(),
			comic_chapter_id = commentEntity.comic_chapter_id?.ToString(),
			user_id = commentEntity.user_id.ToString(),
			comment = commentEntity.comment,
			like = commentEntity.like,
			reply_id = commentEntity.reply_id?.ToString(),
			is_rate = commentEntity.is_rate,
			rate_star = commentEntity.rate_star,
			created_at = commentEntity.created_at,
			updated_at = commentEntity.updated_at
		};
	}

	// Update ComicComment entity from UpdateComicCommentRequest
	public static void UpdateFromRequest(this ComicComment commentEntity, UpdateComicCommentRequest request)
	{
		commentEntity.comic_id = request.comic_id.ToSnowflakeId(nameof(request.comic_id));
		commentEntity.comic_chapter_id = request.comic_chapter_id.ToNullableSnowflakeId(nameof(request.comic_chapter_id));
		commentEntity.user_id = request.user_id.ToSnowflakeId(nameof(request.user_id));
		commentEntity.comment = request.comment;
		commentEntity.like = request.like;
		commentEntity.reply_id = request.reply_id.ToNullableSnowflakeId(nameof(request.reply_id));
		commentEntity.is_rate = request.is_rate;
		commentEntity.rate_star = request.rate_star;
		commentEntity.updated_at = DateTime.UtcNow;
	}
}
