---
trigger: always_on
---


# Language

Luôn luôn trả lời bằng tiếng Việt

# Cấm

Nghiêm cấm tự ý tạo file mới có cùng chức năng với 1 file cũ hoặc là bản nâng cấp của một file cũ, Hãy chỉnh sửa trực tiếp lên file đó.
Nghiêm cấm tự ý tạo file mới hoặc chỉnh sửa file không thuộc phạm vi của bạn. Chỉ được phép chỉnh sửa các file được giao.
Nghiêm cấm tự ý tạo file *.md. Hãy hỏi tôi trước khi tạo.
Nghiêm cấm tự ý hardcode.

# CMS Base API - AI Coding Instructions

Đây là dự án ASP.NET Core 8.0 Web API với mô hình kiến trúc, mẫu thiết kế và quy ước code cụ thể. Khi làm việc với dự án, cần tuân thủ các quy tắc sau:

- **ROLES**: Được định nghĩa trong Const\Roles.cs
- **PERMISSION**: Được định nghĩa trong Const\Permissions.cs
- Luôn chỉnh sửa trực tiếp các tệp chứ không tạo file mới như CreateNew thay cho Create
- Phân vùng Areas và các lớp sealed cho các Controller để tổ chức mã nguồn rõ ràng
- Luôn đọc README.md/readme.md trong các thư mục, bao gồm cả các thư mục cha có trong đuờng dẫn tới thư mục đích mà bạn truy cập vào để hiểu các quy ước và mẫu thiết kế cụ thể
- Tìm kiếm tất cả các tệp có khả năng liên quan đến yêu cầu, luôn đọc tất cả các tệp có trong Const/const 
- Đối với các hàm phức tạp, hãy ghi document

## Kiến Trúc Tổng Quan

**Mô Hình**: Areas-based API + Redis caching + Entity Framework Core with PostgreSQL

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

### Xử lý ngoại lệ & Logging

- Sử dụng `RequestException` để ném ra các lỗi có mã trạng thái tùy chỉnh, thông điệp lỗi rõ ràng, thông tin này được dùng để trả về phản hồi API nếu request của người dùng gặp lỗi do request không hợp lệ hoặc lỗi nghiệp vụ
- Sử dụng middleware để xử lý lỗi toàn cục, trả về mã trạng thái và thông điệp lỗi phù hợp
- Ghi log chi tiết cho các lỗi không mong muốn, bao gồm stack trace và thông điệp lỗi

### Cấu Trúc Areas & Routing

Controllers nằm trong `Areas/{Area}/Controllers/{Controller}.cs` CHỈ GỒM 1 TỆP DUY NHẤT:
ĐỐI VỚI CÁC REQUEST CÓ CÙNG MỘT CHỨC NĂNG VÀ KHÔNG YÊU CẦU QUYỀN HẠN RIÊNG BIỆT, SỬ DỤNG CÙNG 1 CONTROLLER VỚI CÁC HTTP METHODS KHÁC NHAU NẰM TRONG "/Controllers/{Controller}.cs" (không tồn tại trong bất kỳ area nào)
Luôn sử dụng snake_case cho JSON properties trong DTOs và schema cơ sở dữ liệu.
Ví dụ với `UserController` trong `Admin` area:
```cs
[ApiController]
[Area("Admin")]
[Authorize(Roles = Role.Admin)]
[Route("Admin/[controller]")]
	public class UserController : ControllerBase
	{
		private readonly Services.IUserService _userService;
		public UserController(Services.IUserService userService)
		{
			_userService = userService;
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

**BE Dev URLs**: http://localhost:44344

### Migrations Cơ Sở Dữ Liệu

```powershell
dotnet ef migrations add {MigrationName}
dotnet ef database update
```

## Quy Ước Dự Án Cụ Thể

### Tổ Chức File

- **Partial Classes**: `public static partial class Extensions` cho extension methods
- **Namespace**: Khớp với cấu trúc thư mục nhưng sử dụng dấu chấm (e.g., `TruyenCV.DTOs.Response`), ngoại trừ Extensions, Const luôn là `TruyenCV`
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
**EF Type Mismatches**: Sử dụng returns kiểu `IEnumerable<T>`, chuyển đổi kiểu rõ ràng cho Redis cache methods
**Cache Serialization**: Thêm `[JsonIgnore]` vào navigation properties để tránh circular references
**Json**: Luôn sử dụng snake_case cho JSON properties trong DTOs và schema cơ sở dữ liệu. Luôn sử dụng Newtonsoft.Json thay cho thư viện mặc định System.Text.Json

## FrontEnd(Next.js) Coding

- tên thư mục đều viết ở dạng lower_case
- tất cả route đều lower_case
- tất cả component đều viết ở dạng PascalCase
- thư mục app/*.tsx là thư mục chứa các tệp tin SSR
- thư mục components/*.tsx là thư mục chứa các component dùng chung, không chứa các tệp tin SSR, chỉ chứa các component tái sử dụng CSR
- Mock Data: đặt nó vào bên trong services, nơi thường dùng để gọi API tới BackEnd
- Tất cả type/interface đều được đặt trong thư mục types/*.ts hoặc types/*.d.ts
- Luôn kiểm tra type/interface trong types trước khi tạo mới

## BackEnd JWT Token Settings
- AccessTokenExpiryMinutes: Thời gian hết hạn của Access Token tính theo phút
- RefreshTokenExpiryDays: Thời gian hết hạn của Refresh Token tính theo ngày
- lấy userId từ Access Token để xác thực người dùng trong các request đến APIController thông qua extension method từ BE/Extensions/Jwt.cs:
```cs
   var userId = User.GetUserId();
```

## Dotnet Entity Framework Core Conventions
- Không update các trường cần tính toán như thế này
```cs
var user = await context.Users.FindAsync(userId);
user.Key -= keyUsed;
context.Users.Update(user);
await context.SaveChangesAsync();```
mà hãy sử dụng
```cs
await context.Users
    .Where(u => u.Id == userId)
    .ExecuteUpdateAsync(setters => setters
        .SetProperty(u => u.Key, u => u.Key - keyUsed));
```
để tránh việc tải toàn bộ entity từ database lên bộ nhớ rồi mới thực hiện cập nhật, cũng như tránh các vấn đề như race condition khi có nhiều request cùng update một entity.
- không bao giờ cập nhật toàn bộ entity nếu chỉ cần cập nhật một vài trường, hãy chỉ định rõ các trường cần cập nhật bằng cách sử dụng ExecuteUpdateAsync với SetProperty như ví dụ trên.
- Không bao giờ sử dụng `.ToList()` hoặc `.ToListAsync()` trừ khi thực sự cần thiết để lấy toàn bộ danh sách vào bộ nhớ. Thay vào đó, hãy sử dụng các phương thức như `FirstOrDefaultAsync()`, `SingleOrDefaultAsync()`, `AnyAsync()`, `CountAsync()`, hoặc `ExecuteUpdateAsync()` để thực hiện các thao tác trực tiếp trên database mà không cần tải toàn bộ dữ liệu về.
- Không bao giờ cập nhật trường update_at,delete_at,create_at thủ công trong code. Hãy để Entity Framework Core tự động quản lý trường này thông qua các thuộc tính cấu hình trong DbContext.
- Không sử dụng Task.WhenAll để chạy các truy vấn database song song. Thay vào đó, hãy thực hiện tuần tự từng truy vấn để tránh các vấn đề về kết nối và hiệu suất.
   ví dụ:
      ```cs
          var comicTasks = comicIds.Select(id => _comicRepository.GetByIdAsync(id));
          var comicResults = await Task.WhenAll(comicTasks);
      ```
    nên được thay thế bằng
        ```cs
          var comicResults = await comicIds.SelectAsync(id => _comicRepository.GetByIdAsync(id));
        ```
## WorkFlow
- Tất cả các yêu cầu liên quan đến Admin Flow và User Flow đều phải được xử lý trong các tệp riêng biệt.
- Mỗi lần có yêu cầu mới liên quan đến Admin Flow hoặc User Flow, hãy mở tệp tương ứng và thực hiện các chỉnh sửa cần thiết.
- Admin: tệp Admin-Flows.md
- User: tệp User-Flows.md