# Entity Framework Query

## Raw SQL

```cs
_dataContext.Database.ExecuteSql(FormattableStringFactory.Create(
      @"CREATE TABLE IF NOT EXISTS APIKEY (
        apiKey TEXT PRIMARY KEY,
        name TEXT,
        server TEXT,
        countLeft INTEGER DEFAULT 0,
        countUsed INTEGER DEFAULT 0,
        EXP TEXT
    );"))
```

## lấy dữ liệu đi kèm theo khoá phụ

```cs
_dataContext.User.Include(m=> m.Fav).FirstOrDefault(m=>m.Id == id && m.Fav.Id == favId);
```

## InitData

```cs
 protected override void OnModelCreating(ModelBuilder modelBuilder)
 {
  modelBuilder.Entity<User>().HasData(
   new User
   {
    Id = 1,
    Fullname = "Quyến",
    Username = "kourain",
    Email = "maiquyen16503@gmail.com",
    PasswordHash = Bcrypt.HashPassword("1408"), // Nên mã hóa mật khẩu trong thực tế, Vd: Bcrypt
   }
  );
 }
```
