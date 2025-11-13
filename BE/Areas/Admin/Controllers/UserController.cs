using TruyenCV.Models;
using TruyenCV.Services;
using TruyenCV.Repositories;
using TruyenCV.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;

namespace TruyenCV.Areas.Admin.Controllers
{
    [ApiController]
    [Area("Admin")]
    [Authorize(Roles = Roles.Admin)]
    [Route("Admin/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IDistributedCache _redisCache;

        public UserController(
            IUserService userService, 
            IRefreshTokenRepository refreshTokenRepository,
            IDistributedCache redisCache)
        {
            _userService = userService;
            _refreshTokenRepository = refreshTokenRepository;
            _redisCache = redisCache;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(long id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            Serilog.Log.Error("Fetched user with ID {UserId} from database", user?.id);
            if (user == null)
            {
                Serilog.Log.Error("Fetching user with ID {UserId} failed", id);
                return NotFound(new { message = "Không tìm thấy người dùng" });
            }

            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] int offset = 0, [FromQuery] int limit = 10, [FromQuery] string? keyword = null)
        {
            var users = await _userService.GetUsersAsync(offset, limit, keyword);
            return Ok(users);
        }

        [HttpGet("{id}/refresh-tokens")]
        public async Task<IActionResult> GetUserRefreshTokens(long id)
        {
            // Kiểm tra user có tồn tại không
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng" });
            }

            var tokens = await _refreshTokenRepository.GetByUserIdAsync(id);

            // Chuyển đổi RefreshToken thành DTO trước khi trả về
            var tokenResponses = tokens.Select(t => new
            {
                id = t.id.ToString(),
                token = t.token,
                expires_at = t.expires_at,
                is_active = t.is_active,
                created_at = t.created_at
            });

            return Ok(tokenResponses);
        }

        [HttpDelete("{id}/refresh-tokens/{tokenId}")]
        public async Task<IActionResult> RevokeUserRefreshToken(long id, long tokenId)
        {
            // Kiểm tra user có tồn tại không
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng" });
            }

            // Lấy token để kiểm tra
            var refreshToken = await _refreshTokenRepository.GetByIdAsync(tokenId);
            if (refreshToken == null)
            {
                return NotFound(new { message = "Không tìm thấy refresh token" });
            }

            // Kiểm tra xem token có thuộc về user không
            if (refreshToken.user_id != id)
            {
                return BadRequest(new { message = "Refresh token không thuộc về người dùng này" });
            }

            // Vô hiệu hóa token
            refreshToken.revoked_at = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(refreshToken);

            return Ok(new { message = "Refresh token đã bị vô hiệu hóa" });
        }

        [HttpDelete("{id}/refresh-tokens")]
        public async Task<IActionResult> RevokeAllUserRefreshTokens(long id)
        {
            // Kiểm tra user có tồn tại không
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng" });
            }

            var count = await _refreshTokenRepository.RevokeAllUserTokensAsync(id);

            return Ok(new { message = $"Đã vô hiệu hóa {count} refresh token" });
        }
    }
}