using System;
using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;
using TruyenCV.Models;

namespace TruyenCV;

public static partial class Extensions
{
    public static UserHasSubscription ToEntity(this CreateUserSubscriptionRequest request)
    {
        return new UserHasSubscription
        {
            user_id = long.Parse(request.user_id),
            subscription_id = long.Parse(request.subscription_id),
            start_at = request.start_at ?? DateTime.UtcNow,
            end_at = request.end_at,
            is_active = request.is_active,
            auto_renew = request.auto_renew,
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };
    }

    public static UserHasSubscription UpdateFromRequest(this UserHasSubscription entity, UpdateUserSubscriptionRequest request)
    {
        entity.end_at = request.end_at;
        entity.is_active = request.is_active;
        entity.auto_renew = request.auto_renew;
        entity.updated_at = DateTime.UtcNow;
        return entity;
    }

    public static UserSubscriptionResponse ToRespDTO(this UserHasSubscription entity)
    {
        return new UserSubscriptionResponse
        {
            id = entity._id,
            user_id = entity.user_id.ToString(),
            subscription_id = entity.subscription_id.ToString(),
            start_at = entity.start_at,
            end_at = entity.end_at,
            is_active = entity.is_active,
            auto_renew = entity.auto_renew,
            created_at = entity.created_at,
            updated_at = entity.updated_at
        };
    }
}
