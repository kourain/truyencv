# Service Manager Web Application

Ứng dụng web đơn giản để quản lý các dịch vụ từ xa thông qua giao diện web.

## Cấu trúc dự án

```
AllSV/
├── app.py                 # Ứng dụng FastAPI chính
├── command.json           # Cấu hình các dịch vụ
├── requirements.txt       # Thư viện Python cần thiết
├── README.md             # Tài liệu hướng dẫn
├── static/               # File tĩnh
|    ├── css/
|    │   └── style.css     # CSS styling
|    └── js/
|        └── app.js        # JavaScript logic
└── views/                # Template HTML (Jinja2)
    │── index.html        # Giao diện chính
    └── login.html
```

## Tính năng

- 🚀 **Khởi chạy dịch vụ**: Khởi động các dịch vụ được cấu hình
- ⏹️ **Dừng dịch vụ**: Dừng các dịch vụ đang chạy một cách an toàn
- 🔄 **Khởi động lại**: Khởi động lại dịch vụ (dừng và khởi chạy lại)
- 📊 **Kiểm tra trạng thái**: Xem trạng thái và thông tin chi tiết của dịch vụ
- 💻 **Giám sát tài nguyên**: Theo dõi CPU, RAM và các thông số hệ thống
- 🌐 **Giao diện web**: Điều khiển từ xa qua trình duyệt web
- 🎨 **Giao diện đẹp**: Template HTML với CSS và JS riêng biệt
- 📱 **Responsive**: Tương thích với mobile và desktop

## Cài đặt

1. Cài đặt các thư viện cần thiết:

```bash
pip install -r requirements.txt
```

2. Đảm bảo file `command.json` có cấu hình các dịch vụ của bạn

## Cấu hình dịch vụ

Chỉnh sửa file `command.json` để thêm hoặc sửa đổi các dịch vụ:

```json
{
    "version": "1.0",
    "commands": {
        "tên-dịch-vụ": {
            "command": "lệnh để chạy",
            "workingDirectory": "thư-mục-làm-việc",
            "name": "Tên hiển thị",
            "env": {
                "BIẾN_MÔI_TRƯỜNG": "giá trị"
            }
        }
    }
}
```

### Ví dụ cấu hình

```json
{
    "version": "1.0",
    "commands": {
        "web-server": {
            "command": "python -m http.server 8080",
            "workingDirectory": "~/web",
            "name": "Web Server",
            "env": {
                "PORT": "8080"
            }
        },
        "database": {
            "command": "mongod --dbpath ./data",
            "workingDirectory": "~/database",
            "name": "MongoDB Database"
        }
    }
}
```

## Chạy ứng dụng

```bash
uvicorn app:app --host 0.0.0.0 --port 8000 --reload
```

Sau đó mở trình duyệt và truy cập: `http://localhost:8000`

## API Endpoints

### Web Interface

- `GET /` - Giao diện web chính

### Service Management

- `GET /services` - Lấy danh sách tất cả dịch vụ
- `GET /services/{service_id}/status` - Kiểm tra trạng thái dịch vụ
- `GET /services/{service_id}/info` - Lấy thông tin chi tiết dịch vụ
- `POST /services/{service_id}/start` - Khởi chạy dịch vụ
- `POST /services/{service_id}/stop` - Dừng dịch vụ
- `POST /services/{service_id}/restart` - Khởi động lại dịch vụ

### System Information

- `GET /system/info` - Lấy thông tin tài nguyên hệ thống

## Cách sử dụng

1. **Khởi chạy ứng dụng**:
Chạy `uvicorn app:app --host localhost --port 8000 --reload`
[Windows] `python -m uvicorn app:app --host localhost --port 8000 --reload`
2. **Mở trình duyệt**: Truy cập `http://localhost:8000`
3. **Quản lý dịch vụ**:
   - Nhấn "Khởi chạy" để bắt đầu một dịch vụ
   - Nhấn "Dừng" để dừng dịch vụ
   - Nhấn "Khởi động lại" để khởi động lại
   - Nhấn "Chi tiết" để xem thông tin tiến trình
4. **Làm mới**: Nhấn "Làm mới" để cập nhật trạng thái

## Tính năng bảo mật

⚠️ **Lưu ý**: Ứng dụng này được thiết kế để sử dụng trong môi trường tin cậy. Trong môi trường production, nên thêm:

- Xác thực người dùng
- HTTPS
- Giới hạn quyền truy cập
- Logging và auditing

## Troubleshooting

### Dịch vụ không khởi chạy được

- Kiểm tra lệnh trong `command.json` có đúng không
- Kiểm tra đường dẫn `workingDirectory` có tồn tại không
- Xem log trong terminal để biết lỗi chi tiết

### Không thể dừng dịch vụ

- Ứng dụng sẽ thử dừng nhẹ nhàng trước, sau đó ép buộc nếu cần
- Một số dịch vụ có thể cần thời gian để dừng hoàn toàn

### Lỗi cổng đã được sử dụng

```bash
uvicorn app:app --host 0.0.0.0 --port 8001 --reload
```

## Yêu cầu hệ thống

- Python 3.7+
- Windows/Linux/macOS
- Quyền truy cập để khởi chạy/dừng tiến trình

## Termux Android

```bash
pkg upgrade
pkg install python uv
source termux.sh
```
