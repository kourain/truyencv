# Table Models

## class

Tất cả Entity class đều phải được kế thừa từ class TruyenCV.Models.BaseEntity

## Coding

Mỗt khi tạo Entity mới hoặc có thay đổi, bắt buộc phải tạo hoặc cập nhật Repository("../Repositories"), Service("../Services"), Controller("../Controlers","../Areas/Admin/Controllers","../Areas/User/Controllers"), DTOs("../DTOs") tương ứng. Cuối cùng hãy chỉnh sửa các types có trong FrontEnd thư mục src/types tương ứng

## SeedData

tạo tệp Entity tương ứng .cs và đặt vào đó, gọi SeedData thông qua AppDataContext.OnModelCreating

## Các FK cấu hình phức tạp

nếu Entity sử dụng nhiều lần 1 Entity khác như:
```cs
public class UserHasRole : BaseEntity
{
	[Required]
	public required string role_name { get; set; }
    [Required]
    public required long user_id {get;set;}
    [Required]
    public required long assigned_by {get;set;}
    [JsonIgnore]
    public virtual User? User {get;set;}
    [JsonIgnore]
    public virtual User? AssignedBy {get;set;}

    public DateTime? revoked_at { get; set; }

    [NotMapped]
    public bool is_active => revoked_at == null && deleted_at == null;
}
```
Hãy đặt nó vào bên trong file Entity tương ứng .cs thuộc thư mục ConfigHardFKey

## [Required] attribute và required

[Required] khiến trường trong bảng được tạo ra từ ``dotnet ef migrate`` ``not null``
required khiến thuộc tính trong đối tượng tạo ra từ class này không thể mang giá trị null

```cs
 [Required]
 public required string UserName { get; set; }
```

## TableName

```cs
[Table("Users")]
```

## PK

```cs
[PrimaryKey(nameof(id))]
```

## Tạo Index

Ví dụ

```cs
[Index(nameof(id), IsUnique = true)]
```

## properties không tồn tại trong db, vd: IsValid

```cs
  [NotMapped]
  public bool IsValid => EndDate < DateTime.UTCNow;
```

## Khoá phụ

Nên sử dụng Nullable (aka: T?) khi tạo liên kết, khi cần sử dụng hãy dùng [Include](../EF-Query.md#lấy-dữ-liệu-đi-kèm-theo-khoá-phụ)

### One - Many

```cs
 [ForeignKey(nameof(id))] // id : PK
 public virtual ICollection<Fav>? Favs { get; set; }
```

### Many/One - One

```cs
 [ForeignKey(nameof(id))] // id : PK
 public virtual Fav? Favs { get; set; }
```
