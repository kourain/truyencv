using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using TruyenCV;
using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;
using TruyenCV.Models;
using TruyenCV.Repositories;

namespace TruyenCV.Services;

public class UserHasSubscriptionService : IUserHasSubscriptionService
{
    private readonly IUserHasSubscriptionRepository _userHasSubscriptionRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserUseKeyHistoryRepository _userUseKeyHistoryRepository;
    private readonly AppDataContext _dbcontext;
    private readonly IDistributedCache _redisCache;

    public UserHasSubscriptionService(
        IUserHasSubscriptionRepository userHasSubscriptionRepository,
        IUserRepository userRepository,
        ISubscriptionRepository subscriptionRepository,
        IUserUseKeyHistoryRepository userUseKeyHistoryRepository,
        AppDataContext dbcontext,
        IDistributedCache redisCache)
    {
        _userHasSubscriptionRepository = userHasSubscriptionRepository;
        _userRepository = userRepository;
        _subscriptionRepository = subscriptionRepository;
        _userUseKeyHistoryRepository = userUseKeyHistoryRepository;
        _dbcontext = dbcontext;
        _redisCache = redisCache;
    }

    public async Task<UserSubscriptionResponse?> GetByIdAsync(long id)
    {
        var subscription = await _userHasSubscriptionRepository.FirstOrDefaultAsync(sub => sub.id == id && sub.deleted_at == null);
        return subscription?.ToRespDTO();
    }

    public async Task<IEnumerable<UserSubscriptionResponse>> GetByUserIdAsync(long userId)
    {
        var subscriptions = await _userHasSubscriptionRepository.GetByUserIdAsync(userId);
        return subscriptions.Where(sub => sub.deleted_at == null).Select(sub => sub.ToRespDTO());
    }

    public async Task<UserSubscriptionResponse> CreateAsync(CreateUserSubscriptionRequest request)
    {
        var userId = request.user_id.ToSnowflakeId(nameof(request.user_id));
        var subscriptionId = request.subscription_id.ToSnowflakeId(nameof(request.subscription_id));

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new UserRequestException("Người dùng không tồn tại");
        }

        var subscription = await _subscriptionRepository.FirstOrDefaultAsync(sub => sub.id == subscriptionId && sub.deleted_at == null);
        if (subscription == null)
        {
            throw new UserRequestException("Gói thuê bao không tồn tại");
        }

        if (!subscription.is_active)
        {
            throw new UserRequestException("Gói thuê bao đã bị vô hiệu hoá");
        }

        var existing = await _userHasSubscriptionRepository.FirstOrDefaultAsync(sub => sub.user_id == userId && sub.subscription_id == subscriptionId && sub.deleted_at == null);
        if (existing != null)
        {
            throw new UserRequestException("Người dùng đã sở hữu gói này");
        }

        using var transaction = await _dbcontext.Database.BeginTransactionAsync();
        try
        {
            var entity = request.ToEntity();
            entity.user_id = userId;
            entity.subscription_id = subscriptionId;

            var created = await _userHasSubscriptionRepository.AddAsync(entity);

            var ticketAdded = subscription.ticket_added;
            if (ticketAdded > 0)
            {
                var keyAdded = (long)ticketAdded;
                user.key += keyAdded;
                user.updated_at = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);

                var historyRequest = new CreateUserUseKeyHistoryRequest
                {
                    user_id = user.id.ToString(),
                    key = keyAdded,
                    status = HistoryStatus.Add,
                    note = $"Nhận vé mở từ gói {subscription.name}"
                };

                await _userUseKeyHistoryRepository.AddAsync(historyRequest.ToEntity());
                await _redisCache.RemoveFromRedisAsync<UserUseKeyHistory>($"user:{userId}");
            }

            await transaction.CommitAsync();

            await _redisCache.RemoveFromRedisAsync<UserHasSubscription>($"user:{userId}");

            return created.ToRespDTO();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<UserSubscriptionResponse?> UpdateAsync(long id, UpdateUserSubscriptionRequest request)
    {
        var subscription = await _userHasSubscriptionRepository.FirstOrDefaultAsync(sub => sub.id == id && sub.deleted_at == null);
        if (subscription == null)
        {
            return null;
        }

        subscription.UpdateFromRequest(request);
        await _userHasSubscriptionRepository.UpdateAsync(subscription);
        await _redisCache.RemoveFromRedisAsync<UserHasSubscription>($"user:{subscription.user_id}");
        return subscription.ToRespDTO();
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var subscription = await _userHasSubscriptionRepository.FirstOrDefaultAsync(sub => sub.id == id && sub.deleted_at == null);
        if (subscription == null)
        {
            return false;
        }

        await _userHasSubscriptionRepository.DeleteAsync(subscription);
        await _redisCache.RemoveFromRedisAsync<UserHasSubscription>($"user:{subscription.user_id}");
        return true;
    }
}
