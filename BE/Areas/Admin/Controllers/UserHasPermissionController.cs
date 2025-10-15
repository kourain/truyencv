using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruyenCV.DTOs.Request;
using TruyenCV.Services;

namespace TruyenCV.Areas.Admin.Controllers;

[ApiController]
[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
[Route("Admin/[controller]")]
public sealed class UserHasPermissionController : ControllerBase
{
    private readonly IUserHasPermissionService _userHasPermissionService;

    public UserHasPermissionController(IUserHasPermissionService userHasPermissionService)
    {
        _userHasPermissionService = userHasPermissionService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _userHasPermissionService.GetUserHasPermissionByIdAsync(id);
        if (result == null)
        {
            return NotFound(new { message = "Không tìm thấy permission" });
        }

        return Ok(result);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(long userId)
    {
        var results = await _userHasPermissionService.GetPermissionsByUserIdAsync(userId);
        return Ok(results);
    }

    [HttpGet("permission/{permission}")]
    public async Task<IActionResult> GetByPermission(Permissions permission)
    {
        var results = await _userHasPermissionService.GetUsersByPermissionAsync(permission);
        return Ok(results);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int offset = 0, [FromQuery] int limit = 10)
    {
        var results = await _userHasPermissionService.GetUserHasPermissionsAsync(offset, limit);
        return Ok(results);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserHasPermissionRequest request)
    {
        try
        {
            var result = await _userHasPermissionService.CreateUserHasPermissionAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.id }, result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateUserHasPermissionRequest request)
    {
        var requestId = request.id.ToSnowflakeId(nameof(request.id));
        if (id != requestId)
        {
            return BadRequest(new { message = "ID không khớp" });
        }

        try
        {
            var result = await _userHasPermissionService.UpdateUserHasPermissionAsync(requestId, request);
            if (result == null)
            {
                return NotFound(new { message = "Không tìm thấy permission" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var deleted = await _userHasPermissionService.DeleteUserHasPermissionAsync(id);
        if (!deleted)
        {
            return NotFound(new { message = "Không tìm thấy permission" });
        }

        return Ok(new { message = "Xóa permission thành công" });
    }
}
