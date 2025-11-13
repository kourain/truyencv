using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruyenCV.Services;

namespace TruyenCV.Areas.User.Controllers;

[ApiController]
[Area("User")]
[Authorize(Roles = Roles.User)]
[Route("User/[controller]")]
public sealed class HomeController : ControllerBase
{
    private readonly IComicService _comicService;

    public HomeController(IComicService comicService)
    {
        _comicService = comicService;
    }
    [HttpGet, Route("/user/home")]
    public async Task<IActionResult> Index()
    {
        if(User.GetUserId() is long userId)
        {
            var featuredComics = await _comicService.GetHomeForUserAsync(userId);
            return Ok(featuredComics);
        }
        return NotFound(new { message = "Không thể xác định người dùng" });
    }
}