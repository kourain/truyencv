using TruyenCV.Models;
using TruyenCV.Services;
using TruyenCV.Repositories;
using TruyenCV.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;

namespace TruyenCV.Areas.User.Controllers
{
    [ApiController]
    [Area("User")]
    [Authorize(Roles = "User")]
    [Route("User/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IDistributedCache _redisCache;

        public ProfileController(
            IUserService userService, 
            IRefreshTokenRepository refreshTokenRepository, 
            IDistributedCache redisCache)
        {
            _userService = userService;
            _refreshTokenRepository = refreshTokenRepository;
            _redisCache = redisCache;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            // Lấy ID user từ claims của token
            ulong? userId = User.GetUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "Không thể xác định người dùng" });
            }

            var user = await _userService.GetUserByIdAsync(userId.Value);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng" });
            }

            return Ok(user);
        }

        [HttpGet("refresh-tokens")]
        public async Task<IActionResult> GetRefreshTokens()
        {
			// Lấy ID user từ claims của token
			ulong? userId = User.GetUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "Không thể xác định người dùng" });
            }
            var tokens = await _refreshTokenRepository.GetByUserIdAsync(userId.Value);

            // Chuyển đổi RefreshToken thành DTO trước khi trả về
            var tokenResponses = tokens.Select(t => new
            {
                id = t.id,
                token = t.token, // Thông thường không nên hiển thị token đầy đủ
                expires_at = t.expires_at,
                is_active = t.is_active,
                created_at = t.created_at
            });

            return Ok(tokenResponses);
        }

        [HttpDelete("refresh-tokens/{tokenId}")]
        public async Task<IActionResult> RevokeRefreshToken(ulong tokenId)
        {
            // Lấy ID user từ claims của token
            ulong? userId = User.GetUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "Không thể xác định người dùng" });
            }

            // Lấy token để kiểm tra xem có thuộc về user không
            var refreshToken = await _refreshTokenRepository.GetByIdAsync(tokenId);
            if (refreshToken == null)
            {
                return NotFound(new { message = "Không tìm thấy refresh token" });
            }

            // Kiểm tra xem token có thuộc về user hiện tại không
            if (refreshToken.user_id != userId.Value)
            {
                return Forbid();
            }

            // Vô hiệu hóa token
            refreshToken.revoked_at = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(refreshToken);

            return Ok(new { message = "Refresh token đã bị vô hiệu hóa" });
        }

        [HttpDelete("refresh-tokens")]
        public async Task<IActionResult> RevokeAllRefreshTokens()
        {
            // Lấy ID user từ claims của token
            ulong? userId = User.GetUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "Không thể xác định người dùng" });
            }

            var count = await _refreshTokenRepository.RevokeAllUserTokensAsync(userId.Value);

            return Ok(new { message = $"Đã vô hiệu hóa {count} refresh token" });
        }
    }
}