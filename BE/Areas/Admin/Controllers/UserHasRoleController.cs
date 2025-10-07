using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using TruyenCV.DTO.Request;
using TruyenCV.Services;

namespace TruyenCV.Areas.Admin.Controllers;

[ApiController]
[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
[Route("Admin/[controller]")]
public sealed class UserHasRoleController : ControllerBase
{
    private readonly IUserHasRoleService _userHasRoleService;
    private readonly IDistributedCache RedisCache;

    public UserHasRoleController(IUserHasRoleService userHasRoleService, IDistributedCache cache)
    {
        _userHasRoleService = userHasRoleService;
        RedisCache = cache;
    }

    /// <summary>
    /// Lấy thông tin user role theo ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var userHasRole = await _userHasRoleService.GetUserHasRoleByIdAsync(id);
        if (userHasRole == null)
            return NotFound(new { message = "Không tìm thấy user role" });

        return Ok(userHasRole);
    }

    /// <summary>
    /// Lấy danh sách role của user
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(long userId)
    {
        var userHasRoles = await _userHasRoleService.GetRolesByUserIdAsync(userId);
        return Ok(userHasRoles);
    }

    /// <summary>
    /// Lấy danh sách user có role cụ thể
    /// </summary>
    [HttpGet("role/{roleName}")]
    public async Task<IActionResult> GetByRoleName(string roleName)
    {
        var userHasRoles = await _userHasRoleService.GetUsersByRoleNameAsync(roleName);
        return Ok(userHasRoles);
    }

    /// <summary>
    /// Lấy danh sách user role với phân trang
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int offset = 0, [FromQuery] int limit = 10)
    {
        var userHasRoles = await _userHasRoleService.GetUserHasRolesAsync(offset, limit);
        return Ok(userHasRoles);
    }

    /// <summary>
    /// Gán role cho user
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserHasRoleRequest request)
    {
        try
        {
            var userHasRole = await _userHasRoleService.CreateUserHasRoleAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = userHasRole.id }, userHasRole);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Cập nhật thông tin user role
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateUserHasRoleRequest request)
    {
        if (id != request.id)
            return BadRequest(new { message = "ID không khớp" });

        try
        {
            var userHasRole = await _userHasRoleService.UpdateUserHasRoleAsync(id, request);
            if (userHasRole == null)
                return NotFound(new { message = "Không tìm thấy user role" });

            return Ok(userHasRole);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Xóa user role
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var result = await _userHasRoleService.DeleteUserHasRoleAsync(id);
        if (!result)
            return NotFound(new { message = "Không tìm thấy user role" });

        return Ok(new { message = "Xóa user role thành công" });
    }
}
