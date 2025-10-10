# Repository Pattern

Thư mục này chứa các repository cho việc truy cập dữ liệu theo mẫu Repository Pattern.

## Cấu Trúc

- `IRepository<T>`: Interface cơ sở cho tất cả các repository
- `Repository<T>`: Lớp cơ sở triển khai `IRepository<T>` với các phương thức CRUD cơ bản
- `I{Entity}Repository`: Interface riêng cho từng entity với các phương thức đặc thù
- `{Entity}Repository`: Lớp triển khai `I{Entity}Repository` với các phương thức đặc thù

## Cấu trúc thư mục

```
├── Interfaces      # chứa các interface
├──> *.cs
├── Implements      # chứa các repository
├──> *.cs
└── Register.cs # nơi đăng ký các repository
```

## Quy Ước sử dụng Redis Caching và Truy vấn

Tất cả các repository đều sử dụng Redis cache pattern từ `Extensions/Redis/Redis.cs`:
### lấy 1 Entity
```csharp
// Lấy entity theo ID
return await _redisCache.GetFromRedisAsync<Entity>(
    _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.id == id),
    $"{id}",
    DefaultCacheMinutes
);
```
### lấy nhiều entity
KHÔNG SỬ DỤNG
```csharp
public async Task<IEnumerable<ComicComment>> GetByChapterIdAsync(ulong chapterId)
{
    return await _redisCache.GetFromRedisAsync<ComicComment>(
        _dbSet.AsNoTracking()
            .Where(c => c.comic_chapter_id == chapterId)
            .OrderByDescending(c => c.created_at)
            .ToListAsync(),
        $"chapter:{chapterId}",
        DefaultCacheMinutes
    );
}
```
Hãy sử dụng
```csharp
public async Task<IEnumerable<ComicComment>> GetByChapterIdAsync(ulong chapterId)
{
    return await _redisCache.GetFromRedisAsync<ComicComment>(
        _dbSet.AsNoTracking()
            .Where(c => c.comic_chapter_id == chapterId)
            .OrderByDescending(c => c.created_at)
            .ToListAsync(),
        $"chapter:{chapterId}",
        DefaultCacheMinutes
    );
}
```
### Có phân trang
```csharp
// Lấy danh sách có phân trang
return await _redisCache.GetFromRedisAsync<Entity>(
    _dbSet.AsNoTracking().Skip(offset).Take(limit).ToListAsync(),
    offset, limit,
    DefaultCacheMinutes
);
```
### Lấy tất cả
```csharp
public async Task<IEnumerable<ComicCategory>> GetAllAsync()
{
    var result = await _redisCache.GetFromRedisAsync<ComicCategory>(
        _dbSet.AsNoTracking().ToListAsync(),
        DefaultCacheMinutes
    );
    return result ?? [];
}
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
public async Task<UserResponse> GetUserById(ulong id)
{
    var user = await _userRepository.GetByIdAsync(id);
    if (user == null)
        throw new NotFoundException("User not found");

    return user.ToRespDTO();
}
```

## Cấm sử dụng
Nghiêm cấm sử dụng `.ContinueWith()` trong repository