# Table Models

## class

Tất cả Entity class đều phải được kế thừa từ class TruyenCV.Models.BaseEntity

## Coding

Mỗt khi tạo Entity mới, bắt buộc phải tạo Repository("../Repositories"), Service("../Services"), Controller("../Controlers","../Areas/Admin/Controllers","../Areas/User/Controllers"), DTOs("../DTOs") tương ứng

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
