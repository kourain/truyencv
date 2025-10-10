using System.Linq;
using Microsoft.Extensions.Caching.Distributed;
using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;
using TruyenCV.Repositories;

namespace TruyenCV.Services;

/// <summary>
/// Implementation của UserHasPermission Service
/// </summary>
public class UserHasPermissionService : IUserHasPermissionService
{
    private readonly IUserHasPermissionRepository _userHasPermissionRepository;
    private readonly IDistributedCache _redisCache;

    public UserHasPermissionService(
        IUserHasPermissionRepository userHasPermissionRepository,
        IDistributedCache redisCache)
    {
        _userHasPermissionRepository = userHasPermissionRepository;
        _redisCache = redisCache;
    }

    public async Task<UserHasPermissionResponse?> GetUserHasPermissionByIdAsync(ulong id)
    {
        var entity = await _userHasPermissionRepository.GetByIdAsync(id);
        return entity?.ToRespDTO();
    }

    public async Task<IEnumerable<UserHasPermissionResponse>> GetPermissionsByUserIdAsync(ulong userId)
    {
        var entities = await _userHasPermissionRepository.GetByUserIdAsync(userId);
        return entities.Select(e => e.ToRespDTO());
    }

    public async Task<IEnumerable<UserHasPermissionResponse>> GetUsersByPermissionAsync(Permissions permission)
    {
        var entities = await _userHasPermissionRepository.GetByPermissionAsync(permission);
        return entities.Select(e => e.ToRespDTO());
    }

    public async Task<IEnumerable<UserHasPermissionResponse>> GetUserHasPermissionsAsync(int offset, int limit)
    {
        var entities = await _userHasPermissionRepository.GetPagedAsync(offset, limit);
        return entities.Select(e => e.ToRespDTO());
    }

    public async Task<UserHasPermissionResponse> CreateUserHasPermissionAsync(CreateUserHasPermissionRequest request)
    {
        var existing = await _userHasPermissionRepository.GetByUserPermissionAsync(request.user_id, request.permissions);
        if (existing != null)
        {
            throw new Exception("User đã có permission này");
        }

        var entity = request.ToEntity();
        var newEntity = await _userHasPermissionRepository.AddAsync(entity);

        await ClearCaches(request.user_id, request.permissions);

        return newEntity.ToRespDTO();
    }

    public async Task<UserHasPermissionResponse?> UpdateUserHasPermissionAsync(ulong id, UpdateUserHasPermissionRequest request)
    {
        var entity = await _userHasPermissionRepository.GetByIdAsync(id);
        if (entity == null)
        {
            return null;
        }

        if (entity.user_id != request.user_id || entity.permissions != request.permissions)
        {
            var duplicate = await _userHasPermissionRepository.GetByUserPermissionAsync(request.user_id, request.permissions);
            if (duplicate != null && duplicate.id != id)
            {
                throw new Exception("User đã có permission này");
            }
        }

        var oldUserId = entity.user_id;
        var oldPermission = entity.permissions;

        entity.UpdateFromRequest(request);
        await _userHasPermissionRepository.UpdateAsync(entity);

        await ClearCaches(oldUserId, oldPermission);
        await ClearCaches(request.user_id, request.permissions);

        return entity.ToRespDTO();
    }

    public async Task<bool> DeleteUserHasPermissionAsync(ulong id)
    {
        var entity = await _userHasPermissionRepository.GetByIdAsync(id);
        if (entity == null)
        {
            return false;
        }

        var userId = entity.user_id;
        var permission = entity.permissions;

    await _userHasPermissionRepository.DeleteAsync(entity);

        await ClearCaches(userId, permission);

        return true;
    }

    private async Task ClearCaches(ulong userId, Permissions permission)
    {
        await _redisCache.RemoveAsync($"UserHasPermission:user:{userId}");
        await _redisCache.RemoveAsync($"UserHasPermission:permission:{(int)permission}");
        await _redisCache.RemoveAsync($"UserHasPermission:one:user:{userId}:permission:{(int)permission}");
    }
}
