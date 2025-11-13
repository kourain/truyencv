using System;
using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;
using TruyenCV.Models;

namespace TruyenCV;

public static partial class Extensions
{
    public static Subscription ToEntity(this CreateSubscriptionRequest request)
    {
        return new Subscription
        {
            code = request.code,
            name = request.name,
            description = request.description,
            price_coin = request.price_coin,
            duration_day = request.duration_day,
            is_active = request.is_active,
            ticket_added = request.ticket_added,
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };
    }

    public static Subscription UpdateFromRequest(this Subscription subscription, UpdateSubscriptionRequest request)
    {
        subscription.code = request.code;
        subscription.name = request.name;
        subscription.description = request.description;
        subscription.price_coin = request.price_coin;
        subscription.duration_day = request.duration_day;
        subscription.is_active = request.is_active;
        subscription.ticket_added = request.ticket_added;
        subscription.updated_at = DateTime.UtcNow;
        return subscription;
    }

    public static SubscriptionResponse ToRespDTO(this Subscription subscription)
    {
        return new SubscriptionResponse
        {
            id = subscription._id,
            code = subscription.code,
            name = subscription.name,
            description = subscription.description,
            price_coin = subscription.price_coin,
            ticket_added = subscription.ticket_added,
            duration_day = subscription.duration_day,
            is_active = subscription.is_active,
            created_at = subscription.created_at,
            updated_at = subscription.updated_at
        };
    }
}
