using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;
using TruyenCV.Helpers;
using TruyenCV.Models;
using TruyenCV.Repositories;
using Microsoft.Extensions.Caching.Distributed;

namespace TruyenCV.Services
{
    /// <summary>
    /// Implementation của User Service
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IDistributedCache _redisCache;

        public UserService(IUserRepository userRepository, IDistributedCache redisCache)
        {
            _userRepository = userRepository;
            _redisCache = redisCache;
        }

        public async Task<UserResponse?> GetUserByIdAsync(long id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user?.ToRespDTO();
        }

        public async Task<IEnumerable<UserResponse>> GetUsersAsync(int offset, int limit)
        {
            var users = await _userRepository.GetPagedAsync(offset, limit);
            return users.Select(u => u.ToRespDTO());
        }

        public async Task<UserResponse> CreateUserAsync(CreateUserRequest userRequest)
        {
            // Kiểm tra email đã tồn tại chưa
            if (await _userRepository.ExistsAsync(u => u.email == userRequest.email))
                throw new Exception("Email đã tồn tại");

            // Chuyển đổi từ DTO sang Entity
            var user = userRequest.ToEntity();

            // Hash mật khẩu
            user.password = Bcrypt.HashPassword(userRequest.password);

            // Thêm user vào database
            var newUser = await _userRepository.AddAsync(user);

            // Cập nhật cache
            await _redisCache.AddOrUpdateInRedisAsync(newUser, newUser.id);

            return newUser.ToRespDTO();
        }

        public async Task<UserResponse?> UpdateUserAsync(long id, UpdateUserRequest userRequest)
        {
            // Lấy user từ database
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return null;

            // Cập nhật thông tin
            user.UpdateFromRequest(userRequest);

            // Cập nhật vào database
            await _userRepository.UpdateAsync(user);

            // Cập nhật cache
            await _redisCache.AddOrUpdateInRedisAsync(user, user.id);

            return user.ToRespDTO();
        }

        public async Task<bool> DeleteUserAsync(long id)
        {
            // Lấy user từ database
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return false;

            // Soft delete: cập nhật deleted_at
            user.deleted_at = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            // Cập nhật cache
            await _redisCache.AddOrUpdateInRedisAsync(user, user.id);

            return true;
        }

        public async Task<UserResponse?> AuthenticateAsync(string email, string password)
        {
            // Lấy user từ database
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || user.deleted_at != null)
                return null;

            // Kiểm tra mật khẩu
            if (!Bcrypt.VerifyPassword(password, user.password))
                return null;

            return user.ToRespDTO();
        }
    }
}