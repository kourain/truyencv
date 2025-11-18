using System;
using System.Collections.Generic;
using TruyenCV.DTOs.Response;
using TruyenCV.Models;

namespace TruyenCV.Repositories;

/// <summary>
/// Interface repository cho UserComicReadHistory entity
/// </summary>
public interface IUserComicReadHistoryRepository : IRepository<UserComicReadHistory>
{
    /// <summary>
    /// Lấy bản ghi theo id
    /// </summary>
    Task<UserComicReadHistory?> GetByIdAsync(long id);

    /// <summary>
    /// Lấy bản ghi đọc theo user và comic
    /// </summary>
    Task<UserComicReadHistory?> GetByUserAndComicAsync(long userId, long comicId);

    /// <summary>
    /// Lấy danh sách lịch sử đọc của user
    /// </summary>
    Task<IEnumerable<UserComicReadHistory>> GetByUserIdAsync(long userId, int limit);

    /// <summary>
    /// Lấy danh sách comic đang được đọc nhiều nhất theo khoảng thời gian
    /// </summary>
    /// <param name="fromUtc">Mốc thời gian UTC bắt đầu thống kê</param>
    /// <param name="limit">Số lượng comic cần lấy</param>
    Task<IEnumerable<UserComicReadAggregate>> GetTopByUpdatedAtAsync(DateTime fromUtc, int limit);

    /// <summary>
    /// Lấy tổng số người đã đọc theo danh sách comic
    /// </summary>
    /// <param name="comicIds">Danh sách comic id</param>
    Task<IDictionary<long, long>> GetReaderCountsAsync(IEnumerable<long> comicIds, int month = 3);
}

public class UserComicReadAggregate
{
    public long comic_id { get; set; }
    public long reader_count { get; set; }
    public DateTime last_read_at { get; set; }
    public ComicResponse Comic { get; set; }
    public UserComicReadAggregate(long comic_id, long reader_count, DateTime last_read_at, ComicResponse Comic)
    {
        this.comic_id = comic_id;
        this.reader_count = reader_count;
        this.last_read_at = last_read_at;
        this.Comic = Comic;
    }
}
