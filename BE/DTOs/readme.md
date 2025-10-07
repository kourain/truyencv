# DTO : Data Transfer Object

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
