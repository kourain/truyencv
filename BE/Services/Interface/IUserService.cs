using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;

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
        Task<UserResponse?> GetUserByIdAsync(long id);
        
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
        Task<UserResponse?> UpdateUserAsync(long id, UpdateUserRequest userRequest);
        
        /// <summary>
        /// Xóa người dùng
        /// </summary>
        /// <param name="id">ID của người dùng</param>
        /// <returns>True nếu xóa thành công, ngược lại là False</returns>
        Task<bool> DeleteUserAsync(long id);
        
        /// <summary>
        /// Xác thực người dùng
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="password">Mật khẩu</param>
        /// <returns>Thông tin người dùng nếu xác thực thành công, ngược lại là null</returns>
        Task<UserResponse?> AuthenticateAsync(string email, string password);
    }
}