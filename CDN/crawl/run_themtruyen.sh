#!/bin/bash
# Ví dụ script để chạy themtruyen.py với các bước hướng dẫn

echo "=========================================="
echo "Script Thêm Truyện Hàng Loạt"
echo "=========================================="
echo ""

# Kiểm tra Python
if ! command -v python3 &> /dev/null; then
    echo "❌ Lỗi: Python 3 chưa được cài đặt"
    echo "Vui lòng cài đặt Python 3.8 trở lên"
    exit 1
fi

echo "✓ Python version: $(python3 --version)"

# Kiểm tra thư viện requests
if ! python3 -c "import requests" 2>/dev/null; then
    echo ""
    echo "❌ Thư viện 'requests' chưa được cài đặt"
    echo "Đang cài đặt..."
    pip3 install requests
    if [ $? -ne 0 ]; then
        echo "❌ Lỗi cài đặt thư viện"
        exit 1
    fi
    echo "✓ Đã cài đặt thư viện requests"
fi

# Kiểm tra thư mục truyen
if [ ! -d "truyen" ]; then
    echo ""
    echo "❌ Không tìm thấy thư mục 'truyen/'"
    
    if [ -f "truyen.zip" ]; then
        echo "Tìm thấy file truyen.zip, đang giải nén..."
        unzip -q truyen.zip
        echo "✓ Đã giải nén truyen.zip"
    else
        echo "Vui lòng chạy script trong thư mục CDN/crawl/ hoặc giải nén truyen.zip"
        exit 1
    fi
fi

echo "✓ Thư mục truyen/ tồn tại"

# Kiểm tra API_TOKEN
echo ""
if [ -z "$API_TOKEN" ]; then
    echo "⚠️  Biến môi trường API_TOKEN chưa được thiết lập"
    echo ""
    echo "Để lấy token:"
    echo "1. Đăng nhập vào hệ thống với tài khoản có role 'Converter'"
    echo "2. Copy JWT token từ response hoặc local storage"
    echo "3. Thiết lập biến môi trường:"
    echo ""
    echo "   export API_TOKEN='your-jwt-token-here'"
    echo ""
    read -p "Nhấn Enter để tiếp tục với demo mode (sẽ báo lỗi), hoặc Ctrl+C để thoát..."
else
    echo "✓ API_TOKEN đã được thiết lập"
fi

# Kiểm tra API_BASE_URL
if [ -z "$API_BASE_URL" ]; then
    echo "⚠️  API_BASE_URL chưa thiết lập, sử dụng mặc định: http://localhost:44344"
    export API_BASE_URL="http://localhost:44344"
else
    echo "✓ API_BASE_URL: $API_BASE_URL"
fi

# Đếm số lượng truyện
COMIC_COUNT=$(find truyen/ -mindepth 1 -maxdepth 1 -type d ! -name 'ads' | wc -l)
echo ""
echo "📚 Tìm thấy $COMIC_COUNT truyện trong thư mục truyen/"
echo ""

# Xác nhận trước khi chạy
echo "Bạn sắp upload $COMIC_COUNT truyện lên backend API"
echo "API URL: $API_BASE_URL"
echo ""
read -p "Bạn có muốn tiếp tục? (y/N): " -n 1 -r
echo ""

if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo "Đã hủy."
    exit 0
fi

echo ""
echo "=========================================="
echo "Bắt đầu upload..."
echo "=========================================="
echo ""

# Chạy script
python3 themtruyen.py

# Kiểm tra kết quả
if [ $? -eq 0 ]; then
    echo ""
    echo "✓ Script hoàn thành!"
    echo "Xem log chi tiết tại: themtruyen.log"
else
    echo ""
    echo "❌ Script gặp lỗi!"
    echo "Xem log chi tiết tại: themtruyen.log"
    exit 1
fi
