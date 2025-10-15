# FastAPI Upload Service for Cloudflare R2

Dịch vụ API để tải lên và quản lý file trên Cloudflare R2 CDN.

## Tính năng

- Tải file lên Cloudflare R2
- Xóa file từ R2
- Kiểm tra định dạng file hợp lệ
- CORS middleware
- Hỗ trợ nhiều loại file (hình ảnh, PDF, ZIP, v.v.)

## Cài đặt

1. Clone repository và cài đặt dependencies:

```bash
cd CDN/cloudflare
python -m venv venv
source venv/bin/activate  # Trên Windows: .\venv\Scripts\activate
pip install -r requirements.txt
```

2. Tạo file .env từ .env.example và cấu hình:

```bash
cp .env.example .env
```

Cập nhật các biến môi trường trong .env:

```
CLOUDFLARE_ACCOUNT_ID=your_account_id
CLOUDFLARE_ACCESS_KEY_ID=your_access_key_id
CLOUDFLARE_SECRET_ACCESS_KEY=your_secret_access_key
CLOUDFLARE_BUCKET_NAME=your_bucket_name
API_SECRET_KEY=your_secret_key_for_jwt
ALLOWED_ORIGINS=http://localhost:3000,https://yoursite.com
```

## Chạy server

```bash
uvicorn app.main:app --reload
```

Server sẽ chạy tại http://localhost:8000

## API Endpoints

### Upload File
```
POST /api/files/upload
```

Tải file lên R2. Gửi file qua form-data với key "file".

### Delete File
```
DELETE /api/files/{filename}
```

Xóa file từ R2 theo tên file.

### Health Check
```
GET /health
```

Kiểm tra trạng thái hoạt động của service.

## Swagger Documentation

Truy cập http://localhost:8000/docs để xem tài liệu API chi tiết.

## Các Loại File Hỗ Trợ

- Images: JPEG, PNG, GIF, WebP
- Documents: PDF
- Archives: ZIP
- Text: Plain text, JSON

## Security

- Kiểm tra định dạng file
- CORS protection
- Rate limiting (coming soon)
- File size limits (coming soon)