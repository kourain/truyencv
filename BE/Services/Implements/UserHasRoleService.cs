using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;
using TruyenCV.Models;
using TruyenCV.Repositories;
using Microsoft.Extensions.Caching.Distributed;

namespace TruyenCV.Services;

/// <summary>
/// Implementation của UserHasRole Service
/// </summary>
public class UserHasRoleService : IUserHasRoleService
{
    private readonly IUserHasRoleRepository _userHasRoleRepository;
    private readonly IDistributedCache _redisCache;

    public UserHasRoleService(IUserHasRoleRepository userHasRoleRepository, IDistributedCache redisCache)
    {
        _userHasRoleRepository = userHasRoleRepository;
        _redisCache = redisCache;
    }

    public async Task<UserHasRoleResponse?> GetUserHasRoleByIdAsync(long id)
    {
        var userHasRole = await _userHasRoleRepository.GetByIdAsync(id);
        return userHasRole?.ToRespDTO();
    }

    public async Task<IEnumerable<UserHasRoleResponse>> GetRolesByUserIdAsync(long userId)
    {
        var userHasRoles = await _userHasRoleRepository.GetByUserIdAsync(userId);
        return userHasRoles.Select(r => r.ToRespDTO());
    }

    public async Task<IEnumerable<UserHasRoleResponse>> GetUsersByRoleNameAsync(string roleName)
    {
        var userHasRoles = await _userHasRoleRepository.GetByRoleNameAsync(roleName);
        return userHasRoles.Select(r => r.ToRespDTO());
    }

    public async Task<IEnumerable<UserHasRoleResponse>> GetUserHasRolesAsync(int offset, int limit)
    {
        var userHasRoles = await _userHasRoleRepository.GetPagedAsync(offset, limit);
        return userHasRoles.Select(r => r.ToRespDTO());
    }

    public async Task<UserHasRoleResponse> CreateUserHasRoleAsync(CreateUserHasRoleRequest request)
    {
        // Kiểm tra user đã có role này chưa
        var existingRoles = await _userHasRoleRepository.GetByUserIdAsync(request.user_id);
        if (existingRoles.Any(r => r.role_name == request.role_name))
            throw new Exception("User đã có role này");

        // Chuyển đổi từ DTO sang Entity
        var userHasRole = request.ToEntity();

        // Thêm vào database
        var newUserHasRole = await _userHasRoleRepository.AddAsync(userHasRole);

        // Cập nhật cache
        await _redisCache.AddOrUpdateInRedisAsync(newUserHasRole, newUserHasRole.id);

        // Xóa cache của danh sách role của user
        await _redisCache.RemoveAsync($"UserHasRole:user:{request.user_id}");
        await _redisCache.RemoveAsync($"UserHasRole:role:{request.role_name}");

        return newUserHasRole.ToRespDTO();
    }

    public async Task<UserHasRoleResponse?> UpdateUserHasRoleAsync(long id, UpdateUserHasRoleRequest request)
    {
        // Lấy user role từ database
        var userHasRole = await _userHasRoleRepository.GetByIdAsync(id);
        if (userHasRole == null)
            return null;

        var oldUserId = userHasRole.user_id;
        var oldRoleName = userHasRole.role_name;

        // Cập nhật thông tin
        userHasRole.UpdateFromRequest(request);

        // Cập nhật vào database
        await _userHasRoleRepository.UpdateAsync(userHasRole);

        // Cập nhật cache
        await _redisCache.AddOrUpdateInRedisAsync(userHasRole, userHasRole.id);

        // Xóa cache cũ
        await _redisCache.RemoveAsync($"UserHasRole:user:{oldUserId}");
        await _redisCache.RemoveAsync($"UserHasRole:role:{oldRoleName}");
        await _redisCache.RemoveAsync($"UserHasRole:user:{request.user_id}");
        await _redisCache.RemoveAsync($"UserHasRole:role:{request.role_name}");

        return userHasRole.ToRespDTO();
    }

    public async Task<bool> DeleteUserHasRoleAsync(long id)
    {
        // Lấy user role từ database
        var userHasRole = await _userHasRoleRepository.GetByIdAsync(id);
        if (userHasRole == null)
            return false;

        var userId = userHasRole.user_id;
        var roleName = userHasRole.role_name;

        // Soft delete: cập nhật deleted_at
        userHasRole.deleted_at = DateTime.UtcNow;
        await _userHasRoleRepository.UpdateAsync(userHasRole);

        // Cập nhật cache
        await _redisCache.AddOrUpdateInRedisAsync(userHasRole, userHasRole.id);

        // Xóa cache
        await _redisCache.RemoveAsync($"UserHasRole:user:{userId}");
        await _redisCache.RemoveAsync($"UserHasRole:role:{roleName}");

        return true;
    }
}
