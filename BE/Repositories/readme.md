# Repository Pattern

Thư mục này chứa các repository cho việc truy cập dữ liệu theo mẫu Repository Pattern.

## Cấu Trúc

- `IRepository<T>`: Interface cơ sở cho tất cả các repository
- `Repository<T>`: Lớp cơ sở triển khai `IRepository<T>` với các phương thức CRUD cơ bản
- `I{Entity}Repository`: Interface riêng cho từng entity với các phương thức đặc thù
- `{Entity}Repository`: Lớp triển khai `I{Entity}Repository` với các phương thức đặc thù

## Cấu trúc thư mục

```
├── Interface      # chứa các interface
├──> *.cs      # PUT methods
├── Repository      # chứa các repository
├──> *.cs      # PUT methods
└── Register.cs # nơi đăng ký các repository
```

## Quy Ước Redis Caching

Tất cả các repository đều sử dụng Redis cache pattern từ `Extensions/Redis/Redis.cs`:

```csharp
// Lấy entity theo ID
return await _redisCache.GetFromRedisAsync<Entity>(
    _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.id == id),
    $"{id}",
    DefaultCacheMinutes
);

// Lấy danh sách có phân trang
return await _redisCache.GetFromRedisAsync<Entity>(
    _dbSet.AsNoTracking().Skip(offset).Take(limit).ToListAsync(),
    offset, limit,
    DefaultCacheMinutes
);

// Lấy tất cả
return await _redisCache.GetFromRedisAsync<T>(
   _dbSet.AsNoTracking().ToListAsync(),
   DefaultCacheMinutes
  );
```

## Quy Ước Key Cache

- Entity đơn: `{TypeName}:{Key}` hoặc `{TypeName}:{PropertyName}:{Value}`
- Danh sách: `{TypeName}:many:{Offset}:{Limit}`
- Danh sách theo thuộc tính: `{TypeName}:{PropertyName}:{Value}`
- Tất cả: `{TypeName}:all`

## Sử Dụng Repository

```csharp
// Trong constructor của Service
private readonly IUserRepository _userRepository;

public UserService(IUserRepository userRepository)
{
    _userRepository = userRepository;
}

// Sử dụng trong phương thức
public async Task<UserResponse> GetUserById(long id)
{
    var user = await _userRepository.GetByIdAsync(id);
    if (user == null)
        throw new NotFoundException("User not found");
        
    return user.ToRespDTO();
}
```
