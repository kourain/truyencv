# BackGroundServices

constructor sẽ chỉ được gọi 1 lần nếu đã đăng ký ứng dụng vào application

```cs
  public TemplateBackgroundService(ILogger<TemplateBackgroundService> logger, IServiceProvider serviceProvider)
  {
   _logger = logger;
   _dataContext = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
  }
```

do DataContext không được truyền từ bên ngoài vào, điều này khiến việc ``GetRequiredService<DataContext>`` sẽ trả về ngoại lệ ServiceProvider không tồn tại
-> cần CreateScope() để tạo 1 Scope cho ServiceProvider cho dịch vụ này

## ThreadLock

```cs
  private readonly static object _lock = new object();
```

trong quá trình hoạt động, nếu nhiều request thay đổi tài nguyên công khai của dịch vụ này cùng lúc có thể gây ra lỗi truy cập bộ nhớ -> Except: MemoriesException
-> sử dụng _lock để khoá truy cập cho tới khi action thay đổi tài nguyên hiện tại hoàn tất

```cs
  lock (_lock)
  {
    tasks.Enqueue(task);
  }
```

## Đăng ký dịch vụ

thêm dịch vụ cho application bằng cách thêm dịch vụ đó vào Register.cs
