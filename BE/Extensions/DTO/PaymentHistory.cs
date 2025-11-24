using System;
using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;
using TruyenCV.Models;

namespace TruyenCV;

public static partial class Extensions
{
    public static PaymentHistory ToEntity(this CreatePaymentHistoryRequest request)
    {
        return new PaymentHistory
        {
            user_id = long.Parse(request.user_id),
            amount_coin = request.amount_coin,
            amount_money = request.amount_money,
            payment_method = request.payment_method,
            reference_id = request.reference_id,
            note = request.note
        };
    }

    public static PaymentHistoryResponse ToRespDTO(this PaymentHistory entity, User? user = null)
    {
        return new PaymentHistoryResponse
        {
            id = entity._id,
            user_id = entity.user_id.ToString(),
            amount_coin = entity.amount_coin,
            amount_money = entity.amount_money,
            payment_method = entity.payment_method,
            reference_id = entity.reference_id,
            note = entity.note,
            user_email = user?.email,
            user_name = user?.name,
            created_at = entity.created_at,
            updated_at = entity.updated_at
        };
    }
}
