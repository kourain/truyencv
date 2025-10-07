# Services

Thư mục này chứa các service cho việc xử lý logic nghiệp vụ trong ứng dụng.

## Cấu Trúc

- `Interface/`: Chứa các interface định nghĩa các service
- `Service/`: Chứa các implementation của các interface

## Nguyên Tắc Thiết Kế

1. **Separation of Concerns**: Các service chỉ xử lý logic nghiệp vụ, không trực tiếp truy cập dữ liệu
2. **Dependency Injection**: Các service phụ thuộc vào các repository thông qua interface
3. **Transaction Management**: Các service quản lý transaction khi cần thiết

## Redis Caching

Các service sử dụng repository đã tích hợp Redis cache để tối ưu hiệu suất. Khi có cập nhật dữ liệu, service có trách nhiệm cập nhật cache thông qua repository.

## Đăng Ký Service

Tất cả service được đăng ký tập trung trong `Register.cs` và sử dụng extension method:

```csharp
public static IServiceCollection AddServices(this IServiceCollection services)
{
    services.AddScoped<IAuthService, AuthService>();
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<IRoleService, RoleService>();
    services.AddScoped<IPermissionService, PermissionService>();
    return services;
}
```

## Sử Dụng Service

Service được inject vào controller thông qua constructor:

```csharp
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    
    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(long id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();
            
        return Ok(user);
    }
}
```
