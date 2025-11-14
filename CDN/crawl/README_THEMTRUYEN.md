# Script Thêm Truyện Hàng Loạt

Script Python để tự động thêm truyện và chương vào backend API từ dữ liệu đã crawl trong thư mục `truyen/`.

## Yêu cầu

- Python 3.8 trở lên
- Thư viện `requests`: `pip install requests`
- Access Token với role `Converter` từ backend API
- Dữ liệu truyện trong thư mục `truyen/` (đã được crawl bằng script `crawl_comic.py`)

## Cài đặt

1. Cài đặt thư viện cần thiết:
```bash
pip install requests
```

2. Thiết lập biến môi trường:

**Linux/Mac:**
```bash
export API_BASE_URL="http://localhost:44344"
export API_TOKEN="your-jwt-token-here"
```

**Windows (PowerShell):**
```powershell
$env:API_BASE_URL = "http://localhost:44344"
$env:API_TOKEN = "your-jwt-token-here"
```

**Windows (CMD):**
```cmd
set API_BASE_URL=http://localhost:44344
set API_TOKEN=your-jwt-token-here
```

Hoặc tạo file `.env` từ file mẫu:
```bash
cp .env.example .env
# Sau đó chỉnh sửa file .env với thông tin thực tế
```

## Lấy Access Token

1. Đăng nhập vào hệ thống với tài khoản có role `Converter`
2. Lấy JWT token từ response hoặc local storage
3. Token có dạng: `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`

## Sử dụng

### Chạy script:

```bash
python themtruyen.py
```

### Script sẽ:

1. Quét thư mục `truyen/` để tìm tất cả các truyện
2. Với mỗi truyện:
   - Đọc file `index.json` để lấy thông tin truyện
   - Tạo truyện mới qua API endpoint `/Converter/Comic`
   - Đọc tất cả các file `chap-{số}` hoặc `chap-{số}.txt`
   - Upload từng chương qua API endpoint `/Converter/ComicChapter`
3. Ghi log chi tiết vào file `themtruyen.log` và console

### Cấu hình tùy chọn:

Bạn có thể điều chỉnh các tham số thông qua biến môi trường:

```bash
# URL của backend API
export API_BASE_URL="https://api.truyencv.com"

# JWT Bearer Token (bắt buộc)
export API_TOKEN="your-token-here"

# Độ trễ giữa các request (giây) - tránh quá tải server
export DELAY_BETWEEN_REQUESTS=0.5

# Số lần retry khi request thất bại
export MAX_RETRIES=3
```

## Cấu trúc dữ liệu đầu vào

Thư mục `truyen/` phải có cấu trúc như sau:

```
truyen/
├── ten-truyen-1/
│   ├── index.json          # Thông tin truyện
│   ├── chap-1              # Nội dung chương 1
│   ├── chap-2              # Nội dung chương 2
│   └── ...
├── ten-truyen-2/
│   ├── index.json
│   ├── chap-1
│   └── ...
└── ads/                    # Thư mục này sẽ bị bỏ qua
```

### Format file `index.json`:

```json
{
    "title": "Tên truyện",
    "description": "Mô tả truyện...",
    "author": "Tác giả",
    "cover_url": "https://example.com/cover.jpg",
    "main_category_id": 1001,
    "category_ids": [2002, 3012, 4001],
    "embedded_from": "metruyencv.biz",
    "embedded_from_url": "https://metruyencv.biz/truyen/slug",
    "comic_status": "Còn tiếp"
}
```

### Mapping trạng thái truyện:

- "Còn tiếp" → Continuing (1)
- "Tạm dừng" → Paused (2)
- "Đã dừng" → Stopped (3)
- "Hoàn thành" → Completed (4)
- "Bị cấm" → Banned (5)

## Log

Script sẽ ghi log vào:
- Console (stdout)
- File `themtruyen.log` (với encoding UTF-8)

Các thông tin được log:
- Tiến trình xử lý từng truyện
- Kết quả tạo truyện và chương (thành công/thất bại)
- Lỗi chi tiết nếu có
- Tổng kết cuối cùng

## Xử lý lỗi

Script có các cơ chế xử lý lỗi:

1. **Retry tự động**: Tự động retry khi gặp lỗi tạm thời (429, 500, 502, 503, 504)
2. **Skip on error**: Nếu truyện nào lỗi, sẽ bỏ qua và tiếp tục với truyện tiếp theo
3. **Detailed logging**: Ghi log chi tiết để debug
4. **Timeout handling**: Timeout 30s cho mỗi request

## Lưu ý

- Đảm bảo backend API đang chạy trước khi chạy script
- Token phải có role `Converter` mới có quyền tạo truyện và chương
- Nên chạy thử với một vài truyện trước khi upload hàng loạt
- Delay giữa các request giúp tránh quá tải server
- Script sẽ bỏ qua thư mục `ads/` và các file không hợp lệ

## Ví dụ output:

```
2025-11-14 10:00:00 - INFO - Bắt đầu upload truyện từ thư mục: /path/to/truyen
2025-11-14 10:00:00 - INFO - API Base URL: http://localhost:44344
2025-11-14 10:00:00 - INFO - Tìm thấy 50 truyện

[1/50] Xử lý truyện: cao-vo-cang-muon-tu-cai-tien
================================================================================
Bắt đầu xử lý truyện: Cao Võ Càng Muốn Tu Cái Tiên
================================================================================
2025-11-14 10:00:01 - INFO - Đang tạo truyện: Cao Võ Càng Muốn Tu Cái Tiên
2025-11-14 10:00:02 - INFO - ✓ Tạo truyện thành công: Cao Võ Càng Muốn Tu Cái Tiên (ID: 123456)
2025-11-14 10:00:02 - INFO - Tìm thấy 200 chương, bắt đầu upload...
2025-11-14 10:00:03 - INFO -   Đang tạo chương 1 (ID truyện: 123456)
2025-11-14 10:00:03 - INFO -   ✓ Tạo chương 1 thành công
...

================================================================================
HOÀN THÀNH!
Tổng số truyện: 50
Thành công: 48
Thất bại: 2
================================================================================
```

## Troubleshooting

### Lỗi "Chưa thiết lập API_TOKEN"
- Đảm bảo đã export biến môi trường `API_TOKEN`
- Token phải là JWT token hợp lệ

### Lỗi 401 (Unauthorized)
- Token không hợp lệ hoặc đã hết hạn
- Tài khoản không có role `Converter`

### Lỗi 404 (Not Found)
- Kiểm tra `API_BASE_URL` có đúng không
- Backend API có đang chạy không

### Lỗi "Không tìm thấy thư mục truyen"
- Chạy script trong thư mục `CDN/crawl/`
- Hoặc giải nén file `truyen.zip` trước
