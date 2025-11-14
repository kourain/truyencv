using System;
using System.ComponentModel.DataAnnotations;
using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;
using TruyenCV.Helpers;
using TruyenCV.Models;
using TruyenCV.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Serilog;

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

        public async Task<UserResponse?> GetUserByIdAsync(long id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user?.ToRespDTO();
        }

        public async Task<IEnumerable<UserResponse>> GetUsersAsync(int offset, int limit, string? keyword = null)
        {
            offset = Math.Max(offset, 0);
            limit = Math.Clamp(limit, 1, 100);

            var query = _dbcontext.Users
                .AsNoTracking()
                .Where(u => u.deleted_at == null);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var sanitized = SanitizeKeyword(keyword);
                var pattern = $"%{sanitized}%";
                var hasNumericId = long.TryParse(keyword, out var numericId);

                query = query.Where(u =>
                    (hasNumericId && u.id == numericId)
                    || EF.Functions.ILike(u.email, pattern, "\\")
                    || EF.Functions.ILike(u.name, pattern, "\\"));
            }

            query = query
                .OrderByDescending(u => u.created_at)
                .ThenBy(u => u.id);

            var users = await query
                .Skip(offset)
                .Take(limit)
                .ToListAsync();

            Serilog.Log.Information("Fetched {Count} users (offset: {Offset}, limit: {Limit}, keyword: {Keyword})", users.Count, offset, limit, keyword);

            return users.Select(u => u.ToRespDTO());
        }

        private static string SanitizeKeyword(string keyword)
        {
            return keyword.Trim()
                .Replace("\\", "\\\\")
                .Replace("%", "\\%")
                .Replace("_", "\\_");
        }

        public async Task<UserResponse> CreateUserAsync(CreateUserRequest userRequest)
        {
            // Kiểm tra email đã tồn tại chưa
            if (await _userRepository.ExistsAsync(u => u.email == userRequest.email))
                throw new UserRequestException("Email đã tồn tại");

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

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            // Lấy user từ database
            var user = await _userRepository.GetByEmailAsync(email);
            if (user != null && Bcrypt.VerifyPassword(password, user.password) && user.deleted_at == null)
            {
                return await GetActiveUserWithAccessAsync(user.id);
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

        public async Task UpdatePasswordAsync(long userId, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                throw new UserRequestException("Mật khẩu mới không hợp lệ", nameof(newPassword));
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new UserRequestException("Người dùng không tồn tại");
            }

            user.password = Bcrypt.HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);

            await _redisCache.AddOrUpdateInRedisAsync(user, user.id);
            await _redisCache.RemoveAsync($"User:one:email:{user.email}");
        }

        public async Task<UserProfileResponse?> GetProfileAsync(long userId)
        {
            var user = await _dbcontext.Users
                .AsSplitQuery().IgnoreAutoIncludes()
                .Where(u => u.id == userId && u.deleted_at == null)
                .Include(u => u.Roles.Where(role => role.deleted_at == null))
                .Include(u => u.Permissions.Where(permission => permission.deleted_at == null))
                .Include(u => u.Subscriptions.Where(subscription => subscription.deleted_at == null))
                    .ThenInclude(subscription => subscription.Subscription)
                .FirstOrDefaultAsync();

            return user?.ToProfileDTO();
        }

        public async Task ChangePasswordAsync(long userId, string currentPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(currentPassword))
            {
                throw new UserRequestException("Mật khẩu hiện tại không hợp lệ", nameof(currentPassword));
            }

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                throw new UserRequestException("Mật khẩu mới phải có tối thiểu 6 ký tự", nameof(newPassword));
            }

            var user = await _dbcontext.Users.IgnoreAutoIncludes()
                .Where(u => u.id == userId && u.deleted_at == null)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new UserRequestException("Người dùng không tồn tại");
            }

            if (!Bcrypt.VerifyPassword(currentPassword, user.password))
            {
                throw new UserRequestException("Mật khẩu hiện tại không chính xác", nameof(currentPassword));
            }

            if (Bcrypt.VerifyPassword(newPassword, user.password))
            {
                throw new UserRequestException("Mật khẩu mới phải khác mật khẩu hiện tại", nameof(newPassword));
            }

            user.password = Bcrypt.HashPassword(newPassword);
            user.updated_at = DateTime.UtcNow;

            await _dbcontext.SaveChangesAsync();

            await _redisCache.AddOrUpdateInRedisAsync(user, user.id);
            await _redisCache.RemoveAsync($"User:one:email:{user.email}");
        }

        public async Task<UserProfileResponse?> VerifyEmailAsync(long userId)
        {
            var user = await _dbcontext.Users.IgnoreAutoIncludes()
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

        public async Task<UserProfileResponse?> ChangeEmailAsync(long userId, string newEmail, string? currentPassword)
        {
            var sanitizedEmail = newEmail?.Trim();
            if (string.IsNullOrWhiteSpace(sanitizedEmail))
            {
                throw new UserRequestException("Email mới không được để trống", nameof(newEmail));
            }

            var emailValidator = new EmailAddressAttribute();
            if (!emailValidator.IsValid(sanitizedEmail))
            {
                throw new UserRequestException("Email mới không hợp lệ", nameof(newEmail));
            }

            var user = await _dbcontext.Users
                .IgnoreAutoIncludes()
                .Where(u => u.id == userId && u.deleted_at == null)
                .Include(u => u.Roles.Where(role => role.deleted_at == null))
                .Include(u => u.Permissions.Where(permission => permission.deleted_at == null))
                .Include(u => u.Subscriptions.Where(subscription => subscription.deleted_at == null))
                    .ThenInclude(subscription => subscription.Subscription)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return null;
            }

            if (string.Equals(user.email, sanitizedEmail, StringComparison.OrdinalIgnoreCase))
            {
                throw new UserRequestException("Email mới phải khác email hiện tại", nameof(newEmail));
            }

            var duplicatedEmail = await _dbcontext.Users.AsNoTracking()
                .AnyAsync(u => u.id != userId && u.deleted_at == null && EF.Functions.ILike(u.email, sanitizedEmail));
            if (duplicatedEmail)
            {
                throw new UserRequestException("Email đã được sử dụng", nameof(newEmail));
            }

            var requiresPassword = string.IsNullOrWhiteSpace(user.firebase_uid);
            if (requiresPassword)
            {
                var normalizedCurrentPassword = currentPassword?.Trim();
                if (string.IsNullOrWhiteSpace(normalizedCurrentPassword))
                {
                    throw new UserRequestException("Vui lòng nhập mật khẩu hiện tại", nameof(currentPassword));
                }

                if (!Bcrypt.VerifyPassword(normalizedCurrentPassword, user.password))
                {
                    throw new UserRequestException("Mật khẩu hiện tại không chính xác", nameof(currentPassword));
                }
            }

            var cachedOldEmailKey = $"User:one:email:{user.email}";
            user.email = sanitizedEmail;
            user.email_verified_at = null;
            user.updated_at = DateTime.UtcNow;

            await _dbcontext.SaveChangesAsync();

            await _redisCache.AddOrUpdateInRedisAsync(user, user.id);
            await _redisCache.RemoveAsync(cachedOldEmailKey);
            await _redisCache.RemoveAsync($"User:one:email:{user.email}");

            return user.ToProfileDTO();
        }

        public async Task<User?> GetActiveUserWithAccessAsync(long userId)
        {
            var now = DateTime.UtcNow;

            var us = await _dbcontext.Users
                .AsSplitQuery().IgnoreAutoIncludes()
                .Where(u => u.id == userId && u.deleted_at == null && u.is_banned == false)
                .FirstOrDefaultAsync();
            us.Roles = await _dbcontext.UserHasRoles.Where(u => u.user_id == userId && u.deleted_at == null && (u.revoked_at == null || u.revoked_at < now)).ToListAsync();
            us.Permissions = await _dbcontext.UserHasPermissions.Where(u => u.user_id == userId && u.deleted_at == null && (u.revoked_at == null || u.revoked_at < now)).ToListAsync();
            return us;
        }

        public async Task<UserProfileResponse?> UnlinkFirebaseAsync(long userId)
        {
            var user = await _dbcontext.Users
                .IgnoreAutoIncludes()
                .Where(u => u.id == userId && u.deleted_at == null)
                .Include(u => u.Roles.Where(role => role.deleted_at == null))
                .Include(u => u.Permissions.Where(permission => permission.deleted_at == null))
                .Include(u => u.Subscriptions.Where(subscription => subscription.deleted_at == null))
                    .ThenInclude(subscription => subscription.Subscription)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(user.firebase_uid))
            {
                throw new UserRequestException("Tài khoản chưa liên kết Firebase", nameof(user.firebase_uid));
            }

            var emailCacheKey = $"User:one:email:{user.email}";
            user.firebase_uid = null;
            user.email_verified_at = null;
            user.updated_at = DateTime.UtcNow;

            await _dbcontext.SaveChangesAsync();
            await _redisCache.AddOrUpdateInRedisAsync(user, user.id);
            await _redisCache.RemoveAsync(emailCacheKey);

            return user.ToProfileDTO();
        }
    }
}