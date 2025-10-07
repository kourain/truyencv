# Language

Luôn luôn trả lời bằng tiếng Việt

# Cấm

Nghiêm cấm tự ý tạo file mới hoặc chỉnh sửa file không thuộc phạm vi của bạn. Chỉ được phép chỉnh sửa các file được giao.
Nghiêm cấm tự ý tạo file *.md

# CMS Base API - AI Coding Instructions

Đây là dự án ASP.NET Core 8.0 Web API với mô hình kiến trúc, mẫu thiết kế và quy ước code cụ thể. Khi làm việc với dự án, cần tuân thủ các quy tắc sau:

- **ROLES**: Được định nghĩa trong Const\Enum\Roles.cs
- **PERMISSION**: Được định nghĩa trong Const\Enum\Permission.cs
- Luôn chỉnh sửa trực tiếp các tệp chứ không tạo file mới như CreateNew thay cho Create
- Phân vùng Areas và các lớp sealed cho các Controller để tổ chức mã nguồn rõ ràng
- Luôn đọc README.md/readme.md trong từng thư mục để hiểu các quy ước và mẫu thiết kế cụ thể

## Kiến Trúc Tổng Quan

**Mô Hình**: Areas-based API + Redis caching + Entity Framework Core with MySQL

- **Areas**: `Admin/` và `User/` phân chia controllers theo quyền truy cập API
- **Controllers**: thuộc folder 'Controllers' không thuộc Areas nào, Chứa các endpoints API, sử dụng attribute routing, dành cho các request không yêu cầu phân quyền riêng biệt
- **Services**: Chứa logic nghiệp vụ, tương tác với repositories
- **Repositories**: Chứa các phương thức truy cập dữ liệu, tương tác với DbContext
- **DataContext**: Quản lý kết nối cơ sở dữ liệu và cấu hình
- **Models**: Định nghĩa các entity và cấu hình EF Core
- **DTOs**: Chứa các lớp Data Transfer Objects cho request và response
- **Extensions**: Chứa các extension methods cho mapping DTO, Redis caching, và các tiện ích khác
- **Middleware**: Xử lý các yêu cầu HTTP, bao gồm logging, error handling, và routing dựa trên area
- **Const**: Chứa các hằng số và enum dùng trong toàn bộ ứng dụng
- **Redis Caching**: Sử dụng Redis để cache dữ liệu truy vấn nhằm cải thiện hiệu suất
- **Dependency Injection**: Sử dụng DI để quản lý các dịch vụ, repositories, và DbContext
- **Error Handling**: Middleware để xử lý lỗi toàn cục và trả về phản hồi phù hợp
- **Partial Classes**: Extensions được chia thành nhiều tệp
- **Redis Cache-Aside**: Sử dụng extension methods cho các truy vấn EF với cache tự động
- **DTO Pattern**: Tách biệt Request/Response DTOs với extension methods cho việc mapping

## Quy Tắc Phát Triển Quan Trọng

### Cấu Trúc Areas & Routing

Controllers nằm trong `Areas/{Area}/Controllers/{Controller}.cs` CHỈ GỒM 1 TỆP DUY NHẤT:
ĐỐI VỚI CÁC REQUEST CÓ CÙNG MỘT CHỨC NĂNG VÀ KHÔNG YÊU CẦU QUYỀN HẠN RIÊNG BIỆT, SỬ DỤNG CÙNG 1 CONTROLLER VỚI CÁC HTTP METHODS KHÁC NHAU NẰM TRONG "/Controllers/{Controller}.cs" (không tồn tại trong bất kỳ area nào)
Luôn sử dụng snake_case cho JSON properties trong DTOs và schema cơ sở dữ liệu.
Tất cả các lớp controller đều là partial và được tổ chức như sau:
```
Areas/Admin/Controllers/UserController.cs
├── Get         # GET methods
├── Create      # POST methods
├── Update      # PUT methods
├── Patch      # PATCH methods
└── Delete      # DELETE methods
```
Ví dụ với `UserController` trong `Admin` area:
```cs
[ApiController]
[Area("Admin")]
[Authorize(Roles = Role.Admin)]
[Route("Admin/[controller]")]
	public class UserController : ControllerBase
	{
		private readonly Services.IUserService _userService;
    private readonly IDistributedCache RedisCache;

		public UserController(Services.IUserService userService, IDistributedCache cache)
		{
			_userService = userService;
			RedisCache = cache;
		}

		[HttpGet]
		public async Task<IActionResult> Index(long id)
		{
    // implementation
		}
	}
```

**Routing**: `[Route("/[area]/[controller]")]` - Admin có endpoints khác với User area.

### DTO Mapping Extensions

Chuyển đổi giữa models và DTOs sử dụng extension methods trong `Extensions/DTO/`:

```csharp
// Request DTO -> Model: request.ToEntity()
var user = request.ToEntity();

// Model -> Response DTO: model.ToRespDTO()
return user.ToRespDTO();

// Cập nhật Model từ Request DTO: model.UpdateFromRequest(request)
user.UpdateFromRequest(request);
```

### Quy Ước Entity Framework

- **Models**:
  - PrimaryKey `id` luôn sử dụng long (big int)
  - Sử dụng `[Required]` + `required` cho properties không nullable
  - Tất cả properties trong schema + DTO phải sử dụng snake_case
  - Các thuộc tính DateTime mặc định sử dụng DateTime.UtcNow
  - Giá trị created_at được thiết lập bởi request DTO với `DateTime.UtcNow`, không init trong Models
- **Foreign Keys**: Đánh dấu bằng `[JsonIgnore]` để tránh vòng lặp serialization Redis
- **Indexes**: Luôn thêm `[Index(nameof(Property), IsUnique = true)]` cho trường unique
- **Seed Data**: Sử dụng `OnModelCreating()` trong `DataContext.cs` với `Bcrypt.HashPassword()`
- **Migrations**: Sử dụng `dotnet ef migrations add {Name}` và `dotnet ef database update`
- **Queries**: Luôn sử dụng `AsNoTracking()` cho truy vấn chỉ đọc
- **Async**: Sử dụng async/await cho tất cả EF calls
- **DataContext**: Sử dụng repository/service pattern với DI, không bao giờ `new` trực tiếp
- **Repository/Service Pattern**: Tách biệt logic truy cập dữ liệu (Repository) và logic nghiệp vụ (Service). Controllers chỉ tương tác với Services.
- **Repositiories**: Chứa các phương thức truy cập dữ liệu, sử dụng DbContext để thực hiện các thao tác CRUD.
- **Services**: Chứa logic nghiệp vụ, sử dụng các phương thức từ Repositories

## Workflows Phát Triển

### Chạy Ứng Dụng

```powershell
dotnet watch        # Development với hot reload
dotnet run          # Chạy bình thường
```

**Dev URLs**: http://localhost:44344, https://localhost:7287
**Swagger**: Có sẵn trong môi trường development tại `/swagger`

### Migrations Cơ Sở Dữ Liệu

```powershell
dotnet ef migrations add {MigrationName}
dotnet ef database update
```

## Quy Ước Dự Án Cụ Thể

### Tổ Chức File

- **Partial Classes**: `public static partial class Extensions` cho extension methods
- **Namespace**: Khớp với cấu trúc thư mục nhưng sử dụng dấu chấm (e.g., `TruyenCV.DTO.Response`), ngoại trừ Extensions, Const luôn là `TruyenCV`
- **Documentation**: Vietnamese comments trong markdown files giải thích các patterns

### Extension Methods

Luôn sử dụng `namespace TruyenCV;`
- **Redis**: Tất cả trong `RedisExtensions` partial class
- **DTO Mapping**: Entity-specific extensions trong `Extensions/DTO/{Entity}.cs`
- **Utilities**: Generic extensions trong `Extensions/Linq/` và `Extensions/Other.cs`

### Xử Lý Lỗi

- **Middleware**: `AreaMiddleware` logs route values và handles area-based routing
- **Cache Fallback**: Graceful degradation từ Redis to in-memory cache
- **Nullable**: Extensive null-conditional operators và nullable reference types

## Vấn Đề Thường Gặp & Giải Pháp

**Redis DI Issues**: Đảm bảo `IDistributedCache` được đăng ký - fallback về `AddDistributedMemoryCache()`
**CORS Problems**: Kiểm tra thứ tự middleware - CORS trước UseHttpsRedirection()
**EF Type Mismatches**: Sử dụng returns kiểu `IEnumerable<T>`, chuyển đổi kiểu rõ ràng cho Redis cache methods
**Cache Serialization**: Thêm `[JsonIgnore]` vào navigation properties để tránh circular references
**Json**: Luôn sử dụng snake_case cho JSON properties trong DTOs và schema cơ sở dữ liệu. Luôn sử dụng Newtonsoft.Json thay cho thư viện mặc định System.Text.Json