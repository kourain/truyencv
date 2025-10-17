using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;
using TruyenCV.Repositories;
using Microsoft.Extensions.Caching.Distributed;

namespace TruyenCV.Services;

/// <summary>
/// Implementation của ComicComment Service
/// </summary>
public class ComicCommentService : IComicCommentService
{
	private readonly IComicCommentRepository _commentRepository;
	private readonly IComicRepository _comicRepository;
	private readonly IComicChapterRepository _chapterRepository;
	private readonly IUserRepository _userRepository;
	private readonly IDistributedCache _redisCache;

	public ComicCommentService(
		IComicCommentRepository commentRepository,
		IComicRepository comicRepository,
		IComicChapterRepository chapterRepository,
		IUserRepository userRepository,
		IDistributedCache redisCache)
	{
		_commentRepository = commentRepository;
		_comicRepository = comicRepository;
		_chapterRepository = chapterRepository;
		_userRepository = userRepository;
		_redisCache = redisCache;
	}

	public async Task<ComicCommentResponse?> GetCommentByIdAsync(long id)
	{
		var comment = await _commentRepository.GetByIdAsync(id);
		return comment?.ToRespDTO();
	}

	public async Task<IEnumerable<ComicCommentResponse>> GetCommentsByComicIdAsync(long comicId)
	{
		var comments = await _commentRepository.GetByComicIdAsync(comicId);
		return comments.Select(c => c.ToRespDTO());
	}

	public async Task<IEnumerable<ComicCommentResponse>> GetCommentsByChapterIdAsync(long chapterId)
	{
		var comments = await _commentRepository.GetByChapterIdAsync(chapterId);
		return comments.Select(c => c.ToRespDTO());
	}

	public async Task<IEnumerable<ComicCommentResponse>> GetCommentsByUserIdAsync(long userId)
	{
		var comments = await _commentRepository.GetByUserIdAsync(userId);
		return comments.Select(c => c.ToRespDTO());
	}

	public async Task<IEnumerable<ComicCommentResponse>> GetRepliesAsync(long commentId)
	{
		var replies = await _commentRepository.GetRepliesAsync(commentId);
		return replies.Select(r => r.ToRespDTO());
	}

	public async Task<ComicCommentResponse> CreateCommentAsync(CreateComicCommentRequest commentRequest)
	{
		var comicId = commentRequest.comic_id.ToSnowflakeId(nameof(commentRequest.comic_id));
		var userId = commentRequest.user_id.ToSnowflakeId(nameof(commentRequest.user_id));
		var chapterId = commentRequest.comic_chapter_id.ToNullableSnowflakeId(nameof(commentRequest.comic_chapter_id));
		var replyId = commentRequest.reply_id.ToNullableSnowflakeId(nameof(commentRequest.reply_id));

		// Kiểm tra comic có tồn tại không
		var comic = await _comicRepository.GetByIdAsync(comicId);
		if (comic == null)
			throw new UserRequestException("Comic không tồn tại");

		// Kiểm tra user có tồn tại không
		var user = await _userRepository.GetByIdAsync(userId);
		if (user == null)
			throw new UserRequestException("User không tồn tại");

		// Kiểm tra chapter nếu có
		if (chapterId.HasValue)
		{
			var chapter = await _chapterRepository.GetByIdAsync(chapterId.Value);
			if (chapter == null)
				throw new UserRequestException("Chapter không tồn tại");
		}

		// Kiểm tra comment cha nếu có
		if (replyId.HasValue)
		{
			var parentComment = await _commentRepository.GetByIdAsync(replyId.Value);
			if (parentComment == null)
				throw new UserRequestException("Comment cha không tồn tại");
		}

		// Chuyển đổi từ DTO sang Entity
		var comment = commentRequest.ToEntity();

		// Thêm comment vào database
		var newComment = await _commentRepository.AddAsync(comment);

		// Cập nhật cache
		await _redisCache.AddOrUpdateInRedisAsync(newComment, newComment.id);

		return newComment.ToRespDTO();
	}

	public async Task<ComicCommentResponse?> UpdateCommentAsync(long id, UpdateComicCommentRequest commentRequest)
	{
		// Lấy comment từ database
		var comment = await _commentRepository.GetByIdAsync(id);
		if (comment == null)
			return null;

		// Cập nhật thông tin
		comment.UpdateFromRequest(commentRequest);

		// Cập nhật vào database
		await _commentRepository.UpdateAsync(comment);

		// Cập nhật cache
		await _redisCache.AddOrUpdateInRedisAsync(comment, comment.id);

		return comment.ToRespDTO();
	}

	public async Task<bool> DeleteCommentAsync(long id)
	{
		// Lấy comment từ database
		var comment = await _commentRepository.GetByIdAsync(id);
		if (comment == null)
			return false;

		// Soft delete: cập nhật deleted_at
		comment.deleted_at = DateTime.UtcNow;
		await _commentRepository.UpdateAsync(comment);

		// Cập nhật cache
		await _redisCache.AddOrUpdateInRedisAsync(comment, comment.id);

		return true;
	}
}
