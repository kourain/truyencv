# Controller

## ControllerBase và Controller đã sử dụng trong ASP.NET MVC

ControllerBase: không hỗ trợ View \
Controller: Hỗ trợ View

## sử dụng ControllerBase

Nên đi kèm với APIController

```cs
 [ApiController]
 public class HomeController : ControllerBase
 {
  [Route("/ping")]
  public async Task<IActionResult> Index()
  {
   HttpContext.Response.StatusCode = 200;
   return Ok(new { message = "Pong" });
  }
 }
```

điều này sẽ tạo ra 1 route "/ping" tồn tại đủ tất cả method [GET,POST,PUT,PATCH,DELETE,OPTION,HEAD] \
TUY NHIÊN ĐIỀU NÀY SẼ KHIẾN SwaggerUI KHÔNG THỂ RENDER API ROUTE DOCUMENT ĐÚNG CÁCH

Điều này nên thực hiện như sau:

```cs
 [ApiController]
 public class HomeController : ControllerBase
 {
  //1 method
  [Route("/ping"),HttpGet] // hoặc [HttpGet("/ping")]
  public async Task<IActionResult> Index()
  {
   HttpContext.Response.StatusCode = 200;
   return Ok(new { message = "Pong" });
  }
  //hoặc 2 method
  //1 method
  [Route("/ping"),HttpGet,HttpPost] // hoặc [HttpGet("/ping")]
  public async Task<IActionResult> Index()
  {
   HttpContext.Response.StatusCode = 200;
   return Ok(new { message = "Pong" });
  }
 }
```

## API Return

``await/async`` với ``async Task<T>`` \
kiểu dữ liệu trả về với list hãy sử dụng ``IEnumerable<T>``

```cs
  [Route("/ping"),HttpGet,HttpPost] // hoặc [HttpGet("/ping")]
  public async Task<User> Index()
  {
    Dbcontext.Users.FirstOrDefault(m => m.Id == id);
  }
  //hoặc 1 List
  [Route("/ping"),HttpGet,HttpPost] // hoặc [HttpGet("/ping")]
  public async Task<IEnumerable<User>> Index()
  {
    Dbcontext.Users.Skip(10).Take(50); // bỏ qua 10 user lấy 50 users
  }
```
