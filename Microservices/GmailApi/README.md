# Gmail Bulk Sender

Ứng dụng web cục bộ để gửi email hàng loạt qua Gmail API một cách đơn giản và hiệu quả.

## Tính năng chính

- 🔐 **Xác thực an toàn**: Sử dụng OAuth2 của Google
- 📧 **Gửi hàng loạt**: Gửi email đến hàng trăm người nhận
- 📊 **Theo dõi thời gian thực**: Theo dõi trạng thái gửi email chi tiết
- 📁 **Hỗ trợ file**: Import danh sách từ Excel (.xlsx) và CSV
- 🎨 **Email HTML**: Hỗ trợ nội dung HTML và text plain
- ⚙️ **Tùy chỉnh**: Điều chỉnh thời gian delay giữa các email
- 💾 **Auto-save**: Tự động lưu draft khi soạn email

## Yêu cầu hệ thống

- Python 3.7+
- Gmail account với API enabled
- Google Cloud Project với Gmail API enabled

## Cài đặt

1. **Clone hoặc tải project**:
   ```bash
   git clone <repository-url>
   cd GmailApi
   ```

2. **Cài đặt dependencies**:
   ```bash
   pip install flask google-auth google-auth-oauthlib google-auth-httplib2 google-api-python-client pandas openpyxl
   ```

3. **Cấu hình Google Cloud Project**:
   - Truy cập [Google Cloud Console](https://console.cloud.google.com/)
   - Tạo project mới hoặc chọn project existing
   - Enable Gmail API
   - Tạo OAuth 2.0 credentials (Web application)
   - Thêm `http://localhost:5000/callback` vào Authorized redirect URIs
   - Tải file credentials về và đặt tên là `credentials.json`

## Sử dụng

1. **Khởi động ứng dụng**:
   ```bash
   python app.py
   ```

2. **Truy cập ứng dụng**:
   - Mở browser và truy cập: http://localhost:5000
   
3. **Xác thực Gmail**:
   - Nhấn "Xác thực với Gmail"
   - Đăng nhập với Gmail account
   - Cấp quyền cho ứng dụng

4. **Soạn và gửi email**:
   - Vào trang "Soạn email"
   - Nhập thông tin người gửi và tiêu đề
   - Thêm danh sách người nhận (thủ công hoặc upload file)
   - Soạn nội dung email
   - Nhấn "Gửi email hàng loạt"

5. **Theo dõi trạng thái**:
   - Vào trang "Trạng thái" để xem tiến trình gửi email
   - Xuất log chi tiết nếu cần

## Cấu trúc file

```
GmailApi/
├── app.py              # File chính của ứng dụng Flask
├── credentials.json    # Thông tin OAuth2 từ Google Cloud
├── token.json          # Token xác thực (tự động tạo)
├── templates/          # Templates HTML
│   ├── base.html       # Template gốc
│   ├── index.html      # Trang chủ
│   ├── compose.html    # Trang soạn email
│   └── status.html     # Trang theo dõi trạng thái
└── README.md           # File hướng dẫn này
```

## Format file import

### Excel (.xlsx, .xls)
```
Email
example1@gmail.com
example2@gmail.com
example3@gmail.com
```

### CSV
```
email
example1@gmail.com
example2@gmail.com
example3@gmail.com
```

**Lưu ý**: Email phải ở cột đầu tiên hoặc cột có tên "email"

## Giới hạn và lưu ý

- **Rate Limit**: Gmail API có giới hạn số email gửi/ngày
- **Delay**: Nên đặt delay ít nhất 1 giây giữa các email
- **Permissions**: Email người gửi phải được xác thực qua OAuth2
- **Local only**: Ứng dụng chỉ chạy trên máy cục bộ

## Troubleshooting

### 1. Lỗi "Authentication required"
- Kiểm tra file `credentials.json` có đúng format không
- Thực hiện lại bước xác thực Gmail

### 2. Lỗi "Rate limit exceeded"
- Tăng delay time giữa các email
- Giảm số lượng email gửi trong 1 batch

### 3. Lỗi "Invalid credentials"
- Xóa file `token.json` và xác thực lại
- Kiểm tra OAuth settings trong Google Cloud Console

### 4. Lỗi import file
- Kiểm tra format file Excel/CSV
- Đảm bảo cột email có tên đúng hoặc ở vị trí đầu tiên

## Bảo mật

- ⚠️ **Không chia sẻ** file `credentials.json` và `token.json`
- 🔒 Chỉ chạy ứng dụng trên máy cục bộ tin cậy
- 🚫 Không commit credentials lên repository public

## License

MIT License - Sử dụng tự do cho mục đích cá nhân và thương mại.

## Hỗ trợ

Nếu gặp vấn đề, vui lòng tạo issue hoặc liên hệ qua email.
