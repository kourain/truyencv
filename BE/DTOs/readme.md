# DTO : Data Transfer Object

## Luật chung

tất cả id có trong DTO đều có kiểu dữ liệu là string, Lý do: các Snowflake ID (~7.6e17) vượt quá giới hạn 53-bit nên phía client (JavaScript)
khi chuyển đổi từ Entity sang DTOs, hãy sử dụng trường _id (kiểu string) có trong Entity thay vì sử dụng các hàm chuyển đổi
```csharp
public static DTO.Response.UserResponse ToRespDTO(this Models.User user)
{
    return new DTO.Response.UserResponse
    {
        id = user._id,
        name = user.name,
        FullName = user.name,
        email = user.email,
        created_at = user.created_at
    };
}
```
khi chuyển đổi từ Entity sang DTOs, hãy sử dụng trường _id (kiểu string) có trong Entity thay vì sử dụng các hàm chuyển đổi

## Request

  cho các request

- **Request DTOs**:
  - Dùng cho việc tạo và cập nhật resources
  - Ngoại trừ Create, tất cả RequestDTO đều có thuộc tính `id`
- **Mapping**: Sử dụng extension methods để chuyển đổi giữa models và DTOs, với Create hãy sử dụng ToEntity(), với Update DTO hãy sử dụng UpdateFromRequest()

## Response

  cho các phản hồi

- **Response DTOs**: Dùng để trả dữ liệu cho clients
- **Mapping**: Sử dụng extension methods để chuyển đổi giữa models và DTOs, với ToRespDTO()
