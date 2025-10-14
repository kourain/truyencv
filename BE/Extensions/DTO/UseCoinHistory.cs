using System;
using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;
using TruyenCV.Models;

namespace TruyenCV;

public static partial class Extensions
{
    public static UserCoinHistory ToEntity(this CreateUserCoinHistoryRequest request)
    {
        return new UserCoinHistory
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

    public static UserCoinHistoryResponse ToRespDTO(this UserCoinHistory entity)
    {
        return new UserCoinHistoryResponse
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
