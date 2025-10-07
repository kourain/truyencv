# MiddleWare

## A. MiddleWare toàn ứng dụng

### I. contructor MiddleWare

```cs
  public AreaMiddleware(RequestDelegate next, IServiceProvider provider)
  {
   _next = next;
   _db = provider.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
  }
```

điều này sẽ tạo 1 instant DB cho midware, instant này sẽ tồn tại vĩnh viễn khi ứng dụng vẫn còn hoạt động ~ 1 connection

### II. Action MiddWare

#### Basic

```cs
  public async Task InvokeAsync(HttpContext context)
  {
    await _next(context);
  }
```

### với DBContext, thông thường mỗi request sẽ tạo 1 instant cho DBContext, instant này sẽ tồn tại cho tới khi hết vòng đời của request

```cs
  public async Task InvokeAsync(HttpContext context,DataContext _dbcontext)
  {
    //trước khi gọi next hãy sử dụng SaveChange nếu cần thiết
    await _next(context);
  }
```

## B. MiddleWare áp dụng cho controller/action nhất định

```cs
public class MyCustomFilter : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Xử lý trước khi action chạy
        var _db = provider.GetRequiredService<DataContext>(); // nếu cần sử dụng DBContext
        // provider.CreateScope() sẽ tạo 1 instant mới(không cần thiết)
        await next();
        // Xử lý sau khi action chạy
    }
}

// Áp dụng cho controller hoặc action cụ thể
[MyCustomFilter]
public class MyController : ControllerBase
{
    // ...
}
//hoặc
public class MyController : ControllerBase
{
  [MyCustomFilter]
  public async Task<IActionResult> Index()
  {}
}
```

## C. HttpContext

```cs
  context.Request.RouteValues = {
    (string?) area;
    (string) controller=Home;
    (string) action=Index;
  }
```
