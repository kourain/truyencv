using System;
using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;
using TruyenCV.Helpers;
using TruyenCV.Models;
using TruyenCV.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace TruyenCV.Services
{
    /// <summary>
    /// Implementation của User Service
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IDistributedCache _redisCache;
        private readonly AppDataContext _dbcontext;
        public UserService(IUserRepository userRepository, IDistributedCache redisCache, AppDataContext dbcontext)
        {
            _userRepository = userRepository;
            _redisCache = redisCache;
            _dbcontext = dbcontext;
        }

        public async Task<UserResponse?> GetUserByIdAsync(ulong id)
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

        public async Task<UserResponse?> UpdateUserAsync(ulong id, UpdateUserRequest userRequest)
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

        public async Task<bool> DeleteUserAsync(ulong id)
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

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            // Lấy user từ database
            var user = await _userRepository.GetByEmailAsync(email);
            if (user != null && Bcrypt.VerifyPassword(password, user.password) && user.deleted_at == null)
            {
                return await _dbcontext.Users
                    .Where(u => u.id == user.id && u.deleted_at == null)
                    .Include(u => u.Roles.Where(role => role.deleted_at == null))
                    .Include(u => u.Permissions.Where(permission => permission.deleted_at == null))
                    .FirstOrDefaultAsync();
            }
            return null;
        }

        public async Task<User?> GetUserEntityByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task UpdatePasswordAsync(ulong userId, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                throw new ArgumentException("Mật khẩu mới không hợp lệ", nameof(newPassword));
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("Người dùng không tồn tại");
            }

            user.password = Bcrypt.HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);

            await _redisCache.AddOrUpdateInRedisAsync(user, user.id);
            await _redisCache.RemoveAsync($"User:one:email:{user.email}");
        }

        public async Task<UserProfileResponse?> GetProfileAsync(ulong userId)
        {
            var user = await _dbcontext.Users
                .Where(u => u.id == userId && u.deleted_at == null)
                .Include(u => u.Roles.Where(role => role.deleted_at == null))
                .Include(u => u.Permissions.Where(permission => permission.deleted_at == null))
                .FirstOrDefaultAsync();

            return user?.ToProfileDTO();
        }

        public async Task ChangePasswordAsync(ulong userId, string currentPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(currentPassword))
            {
                throw new ArgumentException("Mật khẩu hiện tại không hợp lệ", nameof(currentPassword));
            }

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                throw new ArgumentException("Mật khẩu mới phải có tối thiểu 6 ký tự", nameof(newPassword));
            }

            var user = await _dbcontext.Users
                .Where(u => u.id == userId && u.deleted_at == null)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new Exception("Người dùng không tồn tại");
            }

            if (!Bcrypt.VerifyPassword(currentPassword, user.password))
            {
                throw new ArgumentException("Mật khẩu hiện tại không chính xác", nameof(currentPassword));
            }

            if (Bcrypt.VerifyPassword(newPassword, user.password))
            {
                throw new ArgumentException("Mật khẩu mới phải khác mật khẩu hiện tại", nameof(newPassword));
            }

            user.password = Bcrypt.HashPassword(newPassword);
            user.updated_at = DateTime.UtcNow;

            await _dbcontext.SaveChangesAsync();

            await _redisCache.AddOrUpdateInRedisAsync(user, user.id);
            await _redisCache.RemoveAsync($"User:one:email:{user.email}");
        }

        public async Task<UserProfileResponse?> VerifyEmailAsync(ulong userId)
        {
            var user = await _dbcontext.Users
                .Where(u => u.id == userId && u.deleted_at == null)
                .Include(u => u.Roles.Where(role => role.deleted_at == null))
                .Include(u => u.Permissions.Where(permission => permission.deleted_at == null))
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return null;
            }

            if (user.email_verified_at == null)
            {
                user.email_verified_at = DateTime.UtcNow;
                user.updated_at = DateTime.UtcNow;

                await _dbcontext.SaveChangesAsync();
                await _redisCache.AddOrUpdateInRedisAsync(user, user.id);
            }

            return user.ToProfileDTO();
        }
    }
}