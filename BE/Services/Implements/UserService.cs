using System;
using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;
using TruyenCV.Helpers;
using TruyenCV.Models;
using TruyenCV.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore;

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
            var user = await _dbcontext.Users.FirstOrDefaultAsync(m=> m.email == email);
            if (user != null && Bcrypt.VerifyPassword(password, user.password))
            {
                return await _dbcontext.Users
                    .Include(u => u.Roles)
                    .Include(u => u.Permissions)
                    .FirstOrDefaultAsync(u => u.id == user.id);
            }
            return user;
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
    }
}