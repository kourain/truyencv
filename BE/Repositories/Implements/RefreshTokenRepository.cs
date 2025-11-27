using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using TruyenCV.Models;

namespace TruyenCV.Repositories;

/// <summary>
/// Implementation repository cho RefreshToken entity
/// </summary>
public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
{
    public override double DefaultCacheMinutes => 5; // Mặc định cache 5 phút
    public RefreshTokenRepository(AppDataContext context, IDistributedCache redisCache)
        : base(context, redisCache)
    {
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _redisCache.GetFromRedisAsync<RefreshToken>(
            () => _dbSet.AsNoTracking().FirstOrDefaultAsync(r => r.token == token),
            $"token:{token}",
            DefaultCacheMinutes
        );
    }

    public async Task<IEnumerable<RefreshToken>> GetByUserIdAsync(long userId)
    {
        return await _redisCache.GetFromRedisAsync<RefreshToken>(
            () => _dbSet.AsNoTracking().Where(r => r.user_id == userId && r.revoked_at == null && r.deleted_at == null && r.expires_at > DateTime.UtcNow).ToListAsync(),
            $"user:{userId}",
            DefaultCacheMinutes
        ) ?? [];
    }

    public async Task<bool> RevokeTokenAsync(string token)
    {
        var refreshToken = await _dbSet.FirstOrDefaultAsync(r => r.token == token);
        if (refreshToken == null) return false;
        refreshToken.revoked_at = DateTime.UtcNow;
        await _dbcontext.SaveChangesAsync();
        // Cập nhật cache
        await _redisCache.AddOrUpdateInRedisAsync(refreshToken, $"token:{token}", DefaultCacheMinutes);
        return true;
    }
    public async Task<int> RevokeAllUserTokensAsync(long userId)
    {
        var userTokens = await _dbSet
            .Where(r => r.user_id == userId && r.revoked_at == null)
            .ToListAsync();
        if (userTokens.Count == 0) return 0;
        var now = DateTime.UtcNow;
        await Task.WhenAll(userTokens.Select(async token =>
        {
            token.revoked_at = now;
            // Cập nhật cache cho mỗi token
            await _redisCache.AddOrUpdateInRedisAsync(token, $"token:{token.token}", DefaultCacheMinutes);
        }));
        await _dbcontext.SaveChangesAsync();
        // Xóa cache danh sách token của user
        // Không cần triển khai tại đây, chỉ xóa từng token trong cache
        return userTokens.Count;
    }
}