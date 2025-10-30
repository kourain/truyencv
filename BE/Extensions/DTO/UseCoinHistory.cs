using System;
using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;
using TruyenCV.Models;

namespace TruyenCV;

public static partial class Extensions
{
    public static UserUseCoinHistory ToEntity(this CreateUserUseCoinHistoryRequest request)
    {
        return new UserUseCoinHistory
        {
            user_id = long.Parse(request.user_id),
            coin = request.coin,
            status = request.status,
            reason = request.reason,
            reference_id = string.IsNullOrWhiteSpace(request.reference_id) ? null : long.Parse(request.reference_id),
            reference_type = request.reference_type,
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };
    }

    public static UserUseCoinHistoryResponse ToRespDTO(this UserUseCoinHistory entity)
    {
        return new UserUseCoinHistoryResponse
        {
            id = entity._id,
            user_id = entity.user_id.ToString(),
            coin = entity.coin,
            status = entity.status,
            reason = entity.reason,
            reference_id = entity.reference_id?.ToString(),
            reference_type = entity.reference_type,
            created_at = entity.created_at,
            updated_at = entity.updated_at
        };
    }
}
