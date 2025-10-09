using TruyenCV.DTO.Request;
using TruyenCV.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TruyenCV.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        /// <summary>
        /// API đăng nhập - trả về access token và refresh token
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Xác thực thông tin đăng nhập
            var user = await _userService.AuthenticateAsync(request.email, request.password);
            if (user == null)
            {
                return Unauthorized(new { message = "Email hoặc mật khẩu không chính xác" });
            }

            // Lấy vai trò của người dùng
            var roles = await _authService.GetUserRolesAsync(user.id);

            // Tạo token
            var (accessToken, refreshToken) = await _authService.GenerateTokensAsync(user, roles);

            // Trả về thông tin
            return Ok(new
            {
                access_token = accessToken,
                refresh_token = refreshToken,
                user = user
            });
        }

        /// <summary>
        /// API làm mới token - sử dụng refresh token để lấy access token mới
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var result = await _authService.RefreshTokenAsync(request.refresh_token);
            if (result == null)
            {
                return Unauthorized(new { message = "Refresh token không hợp lệ hoặc đã hết hạn" });
            }

            var (accessToken, refreshToken) = result.Value;

            return Ok(new
            {
                access_token = accessToken,
                refresh_token = refreshToken
            });
        }

        /// <summary>
        /// API đăng xuất - vô hiệu hóa refresh token
        /// </summary>
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
        {
            var result = await _authService.RevokeRefreshTokenAsync(request.refresh_token);
            if (!result)
            {
                return BadRequest(new { message = "Refresh token không hợp lệ hoặc đã bị vô hiệu hóa" });
            }

            return Ok(new { message = "Đăng xuất thành công" });
        }

        /// <summary>
        /// API đăng xuất khỏi tất cả các thiết bị - vô hiệu hóa tất cả refresh token
        /// </summary>
        [Authorize]
        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAll()
        {
            // Lấy ID user từ claims của token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized(new { message = "Không thể xác định người dùng" });
            }

            var id = long.Parse(userId);
            await _authService.RevokeAllUserTokensAsync(id);

            return Ok(new { message = "Đã đăng xuất khỏi tất cả thiết bị" });
        }
    }
}