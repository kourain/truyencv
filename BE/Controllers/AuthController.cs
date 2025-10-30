using TruyenCV.DTOs.Request;
using TruyenCV.Services;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using TruyenCV.Helpers;
using Serilog;

namespace TruyenCV.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IUserHasRoleService _userHasRoleService;
        private readonly IPasswordResetService _passwordResetService;
        private readonly IFirebaseAuthService _firebaseAuthService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            IUserService userService,
            IUserHasRoleService userHasRoleService,
            IFirebaseAuthService firebaseAuthService,
            IPasswordResetService passwordResetService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _userService = userService;
            _userHasRoleService = userHasRoleService;
            _passwordResetService = passwordResetService;
            _firebaseAuthService = firebaseAuthService;
            _logger = logger;
        }

        /// <summary>
        /// API lấy thông tin tài khoản hiện tại
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.GetUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "Không thể xác định người dùng" });
            }

            var profile = await _userService.GetProfileAsync(userId.Value);
            if (profile == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng" });
            }

            return Ok(profile);
        }

        /// <summary>
        /// API đăng ký người dùng mới - trả về access token và refresh token sau khi tạo tài khoản
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newUser = await _userService.CreateUserAsync(new CreateUserRequest
                {
                    name = request.name,
                    user_name = request.user_name,
                    email = request.email,
                    password = request.password,
                    phone = request.phone
                });

                // Gán quyền mặc định cho người dùng mới
                await _userHasRoleService.CreateUserHasRoleAsync(new CreateUserHasRoleRequest
                {
                    role_name = Roles.User,
                    user_id = newUser.id,
                    assigned_by = SystemUser.id.ToString()
                });

                var userEntity = await _userService.AuthenticateAsync(request.email, request.password);
                if (userEntity == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Không thể xác thực tài khoản vừa đăng ký" });
                }

                var (accessToken, refreshToken) = await _authService.GenerateTokensAsync(userEntity);

                return Ok(new
                {
                    access_token = accessToken,
                    refresh_token = refreshToken,
                    user = newUser,
                    roles = (userEntity.Roles ?? Enumerable.Empty<Models.UserHasRole>())
                        .Where(role => role.deleted_at == null)
                        .Select(role => role.role_name),
                    permissions = (userEntity.Permissions ?? Enumerable.Empty<Models.UserHasPermission>())
                        .Where(permission => permission.deleted_at == null)
                        .Select(permission => permission.permissions.ToString()),
                    access_token_minutes = JwtHelper.AccessTokenExpiryMinutes,
                    refresh_token_days = JwtHelper.RefreshTokenExpiryDays
                });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Email đã tồn tại", StringComparison.OrdinalIgnoreCase))
                {
                    return Conflict(new { message = ex.Message });
                }

                return BadRequest(new { message = ex.Message });
            }
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
                return BadRequest(new { message = "Email hoặc mật khẩu không chính xác" });
            }

            var (accessToken, refreshToken) = await _authService.GenerateTokensAsync(user);
            // Trả về thông tin
            return Ok(new
            {
                access_token = accessToken,
                refresh_token = refreshToken,
                access_token_minutes = JwtHelper.AccessTokenExpiryMinutes,
                refresh_token_days = JwtHelper.RefreshTokenExpiryDays,
                user,
                roles = (user.Roles ?? Enumerable.Empty<Models.UserHasRole>())
                    .Where(role => role.deleted_at == null)
                    .Select(role => role.role_name),
                permissions = (user.Permissions ?? Enumerable.Empty<Models.UserHasPermission>())
                    .Where(permission => permission.deleted_at == null)
                    .Select(permission => permission.permissions.ToString())
            });
        }

        /// <summary>
        /// Đăng nhập bằng Firebase ID token
        /// </summary>
        [HttpPost("firebase-login")]
        [AllowAnonymous]
        public async Task<IActionResult> FirebaseLogin([FromBody] FirebaseLoginRequest request)
        {
            try
            {
                var user = await _firebaseAuthService.SignInWithFirebaseAsync(request);
                var (accessToken, refreshToken) = await _authService.GenerateTokensAsync(user);

                return Ok(new
                {
                    access_token = accessToken,
                    refresh_token = refreshToken,
                    access_token_minutes = JwtHelper.AccessTokenExpiryMinutes,
                    refresh_token_days = JwtHelper.RefreshTokenExpiryDays,
                    user,
                    roles = (user.Roles ?? Enumerable.Empty<Models.UserHasRole>())
                        .Where(role => role.deleted_at == null)
                        .Select(role => role.role_name),
                    permissions = (user.Permissions ?? Enumerable.Empty<Models.UserHasPermission>())
                        .Where(permission => permission.deleted_at == null)
                        .Select(permission => permission.permissions.ToString())
                });
            }
            catch (UserRequestException exception)
            {
                var statusCode = exception.StatusCode == 0 ? StatusCodes.Status400BadRequest : exception.StatusCode;
                return StatusCode(statusCode, new { message = exception.Message.Trim() });
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Không thể đăng nhập bằng Firebase");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Không thể đăng nhập bằng Firebase." });
            }
        }

        /// <summary>
        /// API làm mới token - sử dụng refresh token để lấy access token mới
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            Log.Information($"Refreshing token for user {request.refresh_token}");
            var result = await _authService.RefreshTokenAsync(request.refresh_token);
            if (result == null)
            {
                return Unauthorized(new { message = "Refresh token không hợp lệ hoặc đã hết hạn" });
            }

            var (accessToken, refreshToken) = result.Value;

            return Ok(new
            {
                access_token = accessToken,
                refresh_token = refreshToken,
                access_token_minutes = JwtHelper.AccessTokenExpiryMinutes,
                refresh_token_days = JwtHelper.RefreshTokenExpiryDays
            });
        }

        /// <summary>
        /// API đăng xuất - vô hiệu hóa refresh token
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
        {
            var result = await _authService.RevokeRefreshTokenAsync(request.refresh_token);
            if (!result)
            {
                return Unauthorized(new { message = "Refresh token không hợp lệ hoặc đã bị vô hiệu hóa" });
            }

            return Ok(new { message = "Đăng xuất thành công" });
        }

        /// <summary>
        /// API đăng xuất khỏi tất cả các thiết bị - vô hiệu hóa tất cả refresh token ngoại trừ phiên hiện tại (nếu được cung cấp)
        /// </summary>
        [Authorize]
        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAll([FromBody] RefreshTokenRequest? request)
        {
            // Lấy ID user từ claims của token
            var userId = User.GetUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "Không thể xác định người dùng" });
            }

            var currentRefreshToken = request?.refresh_token;

            await _authService.RevokeAllUserTokensAsync(userId.Value, currentRefreshToken);

            var message = string.IsNullOrWhiteSpace(currentRefreshToken)
                ? "Đã đăng xuất khỏi tất cả thiết bị"
                : "Đã đăng xuất khỏi tất cả thiết bị, trừ phiên hiện tại";

            return Ok(new { message });
        }

        /// <summary>
        /// Yêu cầu gửi OTP đặt lại mật khẩu qua email
        /// </summary>
        [AllowAnonymous]
        [HttpPost("password-reset/request")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userService.GetUserEntityByEmailAsync(request.email);
                if (user != null && user.deleted_at == null)
                {
                    await _passwordResetService.RequestPasswordResetAsync(user.email, user.name);
                }
                else
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(150));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Không thể gửi OTP đặt lại mật khẩu cho {Email}", request.email);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Không thể gửi mã OTP. Vui lòng thử lại sau." });
            }

            return Ok(new { message = "Nếu email tồn tại, mã OTP đã được gửi" });
        }

        /// <summary>
        /// Xác nhận OTP và đặt lại mật khẩu
        /// </summary>
        [AllowAnonymous]
        [HttpPost("password-reset/confirm")]
        public async Task<IActionResult> ConfirmPasswordReset([FromBody] ConfirmPasswordResetRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.GetUserEntityByEmailAsync(request.email);
            if (user == null || user.deleted_at != null)
            {
                await _passwordResetService.InvalidateOtpAsync(request.email);
                return BadRequest(new { message = "OTP không hợp lệ hoặc đã hết hạn" });
            }

            var isValid = await _passwordResetService.ValidateOtpAsync(request.email, request.otp);
            if (!isValid)
            {
                return BadRequest(new { message = "OTP không hợp lệ hoặc đã hết hạn" });
            }

            try
            {
                await _userService.UpdatePasswordAsync(user.id, request.new_password);
                await _authService.RevokeAllUserTokensAsync(user.id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Không thể cập nhật mật khẩu cho {Email}", request.email);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Không thể đặt lại mật khẩu. Vui lòng thử lại sau." });
            }
            finally
            {
                await _passwordResetService.InvalidateOtpAsync(request.email);
            }

            return Ok(new { message = "Đặt lại mật khẩu thành công" });
        }
    }
}