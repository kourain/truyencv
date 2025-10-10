using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;
using TruyenCV.Models;

namespace TruyenCV.Services
{
    /// <summary>
    /// Interface cho User Service
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Lấy thông tin người dùng theo ID
        /// </summary>
        /// <param name="id">ID của người dùng</param>
        /// <returns>Thông tin người dùng</returns>
        Task<UserResponse?> GetUserByIdAsync(ulong id);
        
        /// <summary>
        /// Lấy danh sách người dùng
        /// </summary>
        /// <param name="offset">Vị trí bắt đầu</param>
        /// <param name="limit">Số lượng bản ghi</param>
        /// <returns>Danh sách người dùng</returns>
        Task<IEnumerable<UserResponse>> GetUsersAsync(int offset, int limit);
        
        /// <summary>
        /// Tạo người dùng mới
        /// </summary>
        /// <param name="userRequest">Thông tin người dùng mới</param>
        /// <returns>Thông tin người dùng đã tạo</returns>
        Task<UserResponse> CreateUserAsync(CreateUserRequest userRequest);
        
        /// <summary>
        /// Cập nhật thông tin người dùng
        /// </summary>
        /// <param name="id">ID của người dùng</param>
        /// <param name="userRequest">Thông tin cập nhật</param>
        /// <returns>Thông tin người dùng đã cập nhật</returns>
        Task<UserResponse?> UpdateUserAsync(ulong id, UpdateUserRequest userRequest);
        
        /// <summary>
        /// Xóa người dùng
        /// </summary>
        /// <param name="id">ID của người dùng</param>
        /// <returns>True nếu xóa thành công, ngược lại là False</returns>
        Task<bool> DeleteUserAsync(ulong id);
        
        /// <summary>
        /// Xác thực người dùng
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="password">Mật khẩu</param>
        /// <returns>Thông tin người dùng nếu xác thực thành công, ngược lại là null</returns>
        Task<User?> AuthenticateAsync(string email, string password);

        /// <summary>
        /// Lấy entity người dùng theo email
        /// </summary>
        /// <param name="email">Email của người dùng</param>
        /// <returns>Entity người dùng nếu tồn tại</returns>
        Task<User?> GetUserEntityByEmailAsync(string email);

        /// <summary>
        /// Cập nhật mật khẩu người dùng
        /// </summary>
        /// <param name="userId">ID của người dùng</param>
        /// <param name="newPassword">Mật khẩu mới</param>
        Task UpdatePasswordAsync(ulong userId, string newPassword);
    }
}