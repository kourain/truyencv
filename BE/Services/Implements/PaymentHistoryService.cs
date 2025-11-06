using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;
using TruyenCV.Repositories;
using TruyenCV.Models;

namespace TruyenCV.Services;

public class PaymentHistoryService : IPaymentHistoryService
{
    private readonly IPaymentHistoryRepository _paymentHistoryRepository;
    private readonly IUserRepository _userRepository;

    public PaymentHistoryService(IPaymentHistoryRepository paymentHistoryRepository, IUserRepository userRepository)
    {
        _paymentHistoryRepository = paymentHistoryRepository;
        _userRepository = userRepository;
    }

    public async Task<PaymentHistoryResponse?> GetByIdAsync(long id)
    {
        var history = await _paymentHistoryRepository.FirstOrDefaultAsync(entity => entity.id == id && entity.deleted_at == null);
        return history?.ToRespDTO();
    }

    public async Task<IEnumerable<PaymentHistoryResponse>> GetByUserIdAsync(long userId)
    {
        var histories = await _paymentHistoryRepository.GetByUserIdAsync(userId);
        var user = await _userRepository.GetByIdAsync(userId);
        return histories
            .Where(history => history.deleted_at == null)
            .Select(history => history.ToRespDTO(user))
            .ToList();
    }

    public async Task<IEnumerable<PaymentHistoryResponse>> GetAsync(int offset, int limit, string? keyword = null)
    {
        offset = Math.Max(offset, 0);
        limit = Math.Clamp(limit, 1, 200);

        IEnumerable<PaymentHistory> histories;

        if (string.IsNullOrWhiteSpace(keyword))
        {
            histories = await _paymentHistoryRepository.GetPagedAsync(offset, limit);
        }
        else
        {
            var trimmed = keyword.Trim();
            var userIds = new HashSet<long>();

            if (long.TryParse(trimmed, out var userId))
            {
                userIds.Add(userId);
            }

            var sanitized = SanitizeKeyword(trimmed);
            var pattern = $"%{sanitized}%";
            var matchedUsers = await _userRepository.FindAsync(user => EF.Functions.ILike(user.email, pattern, "\\"));
            foreach (var user in matchedUsers)
            {
                userIds.Add(user.id);
            }

            if (userIds.Count == 0)
            {
                return Array.Empty<PaymentHistoryResponse>();
            }

            histories = await _paymentHistoryRepository.GetPagedByUserIdsAsync(userIds, offset, limit);
        }

        var historiesList = histories.Where(history => history.deleted_at == null).ToList();
        if (historiesList.Count == 0)
        {
            return Array.Empty<PaymentHistoryResponse>();
        }

    var userIdsToLoad = historiesList.Select(history => history.user_id).ToHashSet();
    var userIdsArray = userIdsToLoad.ToArray();
    var relatedUsers = await _userRepository.FindAsync(user => userIdsArray.Contains(user.id));
    var userDict = relatedUsers.ToDictionary(user => user.id, user => user);

        return historiesList.Select(history =>
        {
            userDict.TryGetValue(history.user_id, out var user);
            return history.ToRespDTO(user);
        }).ToList();
    }

    public async Task<PaymentHistoryResponse> CreateAsync(CreatePaymentHistoryRequest request)
    {
        var entity = request.ToEntity();
        var created = await _paymentHistoryRepository.AddAsync(entity);
        return created.ToRespDTO();
    }

    public async Task<IEnumerable<PaymentRevenuePointResponse>> GetRevenueSummaryAsync(int days = 60)
    {
        days = Math.Clamp(days, 1, 180);
        var todayUtc = DateTime.UtcNow.Date;
        var fromDate = todayUtc.AddDays(-(days - 1));
        var toDate = todayUtc.AddDays(1).AddTicks(-1);

        var aggregates = await _paymentHistoryRepository.GetDailyRevenueAsync(fromDate, toDate);
        var aggregateDict = aggregates.ToDictionary(item => item.Date.Date, item => item);

        var results = new List<PaymentRevenuePointResponse>(days);
        for (var current = fromDate; current <= todayUtc; current = current.AddDays(1))
        {
            if (aggregateDict.TryGetValue(current, out var aggregate))
            {
                results.Add(new PaymentRevenuePointResponse
                {
                    date = current.ToString("yyyy-MM-dd"),
                    total_amount_coin = aggregate.TotalCoin,
                    total_amount_money = aggregate.TotalMoney
                });
            }
            else
            {
                results.Add(new PaymentRevenuePointResponse
                {
                    date = current.ToString("yyyy-MM-dd"),
                    total_amount_coin = 0,
                    total_amount_money = 0
                });
            }
        }

        return results;
    }

    private static string SanitizeKeyword(string keyword)
    {
        return keyword
            .Trim()
            .Replace("\\", "\\\\")
            .Replace("%", "\\%")
            .Replace("_", "\\_");
    }
}
