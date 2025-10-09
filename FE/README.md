# Front END website đọc truyện

Cấu trúc thư mục:
|-> public                 # chứa public file như css/js/img/v.v.
|-> src/
|---> app/*.tsx             # cấu trúc giao diện
|---> const/*.ts            # các hằng số sử dụng chung trong ứng dụng
|---> components/*/*.tsx    # các components của giao diện
|---> helpers/*.ts          # các thư viện hỗ trợ, bao gồm cấu hình axios và tanstackquery
|---> hooks/*.ts            # lưu các hook để sử dụng toàn cục
|---> middlewares/*.ts      # middleware
|---> services/*.ts         # api call
|---> types/*.d.ts            # các type được định nghĩa đều nằm ở đây, các type tương tự với BE/DTOs/Response/*.cs và BE/DTOs/Response/*.cs

## Công nghệ

Framework: Next.Js
Ngôn ngữ: TypeScript
ApiCall: TanstackQuery + Axios
Css: Tailwindcss
Đa ngôn ngữ: Không
## Admin

Route: /Admin

## User

Route: /User

## .env

lưu địa chỉ backend
lưu cổng mà front-end sẽ sử dụng để khởi chạy
lưu địa chỉ cdn dùng để tải dữ liệu như nội dung chương truyện, hình ảnh truyện