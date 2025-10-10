using System.Threading.Tasks;
using TruyenCV.DTO.Response;

namespace TruyenCV.Services;

/// <summary>
/// Cung cấp dữ liệu tổng quan cho dashboard quản trị
/// </summary>
public interface IAdminDashboardService
{
	/// <summary>
	/// Lấy dữ liệu tổng quan dashboard
	/// </summary>
	/// <param name="topComics">Số lượng truyện nổi bật</param>
	/// <param name="recentUsers">Số lượng người dùng mới nhất</param>
	/// <param name="categoryLimit">Số lượng danh mục hiển thị</param>
	/// <returns>Thông tin tổng quan dashboard</returns>
	Task<AdminDashboardOverviewResponse> GetOverviewAsync(int topComics = 5, int recentUsers = 6, int categoryLimit = 12);
}
