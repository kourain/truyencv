using System;
using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;
using TruyenCV.Models;

namespace TruyenCV;

public static partial class Extensions
{
    public static UserUseKeyHistory ToEntity(this CreateUserUseKeyHistoryRequest request)
    {
        return new UserUseKeyHistory
        {
            user_id = long.Parse(request.user_id),
            key = request.key,
            status = request.status,
            chapter_id = string.IsNullOrWhiteSpace(request.chapter_id) ? null : long.Parse(request.chapter_id),
            note = request.note,
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };
    }

    public static UserUseKeyHistoryResponse ToRespDTO(this UserUseKeyHistory entity)
    {
        return new UserUseKeyHistoryResponse
        {
            id = entity._id,
            user_id = entity.user_id.ToString(),
            key = entity.key,
            status = entity.status,
            chapter_id = entity.chapter_id?.ToString(),
            note = entity.note,
            created_at = entity.created_at,
            updated_at = entity.updated_at
        };
    }
}
