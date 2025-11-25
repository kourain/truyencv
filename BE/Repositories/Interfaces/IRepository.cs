using System.Linq.Expressions;
using TruyenCV.Models;
using Microsoft.EntityFrameworkCore;

namespace TruyenCV.Repositories;

/// <summary>
/// Interface cơ sở cho tất cả các repository
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    double DefaultCacheMinutes { get; }
    /// <summary>
    /// Lấy tất cả các entities
    /// </summary>
    /// <returns>Danh sách entities</returns>
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(long id);
    /// <summary>
    /// Lấy entities theo điều kiện
    /// </summary>
    /// <param name="expression">Điều kiện filter</param>
    /// <returns>Danh sách entities thỏa mãn điều kiện</returns>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression);

    /// <summary>
    /// Lấy entity theo điều kiện
    /// </summary>
    /// <param name="expression">Điều kiện filter</param>
    /// <returns>Entity đầu tiên thỏa mãn điều kiện</returns>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> expression);

    /// <summary>
    /// Lấy danh sách entities với phân trang
    /// </summary>
    /// <param name="offset">Vị trí bắt đầu</param>
    /// <param name="limit">Số lượng bản ghi</param>
    /// <returns>Danh sách entities</returns>
    Task<IEnumerable<T>> GetPagedAsync(int offset, int limit);

    /// <summary>
    /// Thêm entity mới
    /// </summary>
    /// <param name="entity">Entity cần thêm</param>
    /// <returns>Entity đã thêm</returns>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Cập nhật entity
    /// </summary>
    /// <param name="entity">Entity cần cập nhật</param>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Xóa entity
    /// </summary>
    /// <param name="entity">Entity cần xóa</param>
    Task DeleteAsync(T entity, bool softDelete = true);

    /// <summary>
    /// Đếm số lượng entity
    /// </summary>
    /// <returns>Số lượng entity</returns>
    Task<int> CountAsync();

    /// <summary>
    /// Đếm số lượng entity theo điều kiện
    /// </summary>
    /// <param name="expression">Điều kiện filter</param>
    /// <returns>Số lượng entity thỏa mãn điều kiện</returns>
    Task<int> CountAsync(Expression<Func<T, bool>> expression);

    /// <summary>
    /// Kiểm tra entity tồn tại
    /// </summary>
    /// <param name="expression">Điều kiện filter</param>
    /// <returns>True nếu tồn tại, ngược lại là False</returns>
    Task<bool> ExistsAsync(Expression<Func<T, bool>> expression);
}