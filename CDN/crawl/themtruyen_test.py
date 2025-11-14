"""
Script đơn giản để test upload một vài truyện cụ thể

Sử dụng:
    python themtruyen_test.py slug1 slug2 slug3
    
Ví dụ:
    python themtruyen_test.py tien-bang cao-vo-cang-muon-tu-cai-tien
"""

import sys
import os
from pathlib import Path

# Import module chính
import themtruyen

def main():
    """Test upload với một số truyện được chỉ định"""
    
    if len(sys.argv) < 2:
        print("Sử dụng: python themtruyen_test.py <slug1> [slug2] [slug3] ...")
        print("\nVí dụ:")
        print("  python themtruyen_test.py tien-bang")
        print("  python themtruyen_test.py tien-bang cao-vo-cang-muon-tu-cai-tien")
        print("\nĐể xem danh sách slug có sẵn:")
        print("  ls truyen/")
        sys.exit(1)
    
    # Lấy danh sách slug từ arguments
    comic_slugs = sys.argv[1:]
    
    # Kiểm tra cấu hình
    api_token = os.getenv('API_TOKEN', '')
    if not api_token:
        themtruyen.logger.error("Lỗi: Chưa thiết lập API_TOKEN")
        themtruyen.logger.error("Export biến môi trường: export API_TOKEN='your-token-here'")
        sys.exit(1)
    
    api_base_url = os.getenv('API_BASE_URL', themtruyen.API_BASE_URL)
    truyen_dir = themtruyen.TRUYEN_DIR
    
    if not truyen_dir.exists():
        themtruyen.logger.error(f"Lỗi: Không tìm thấy thư mục {truyen_dir}")
        sys.exit(1)
    
    themtruyen.logger.info(f"Test upload {len(comic_slugs)} truyện")
    themtruyen.logger.info(f"API Base URL: {api_base_url}")
    themtruyen.logger.info("="*80)
    
    # Tạo uploader
    uploader = themtruyen.ComicUploader(api_base_url, api_token)
    
    # Upload từng truyện
    success_count = 0
    failed_count = 0
    
    for i, slug in enumerate(comic_slugs, 1):
        comic_dir = truyen_dir / slug
        
        if not comic_dir.exists():
            themtruyen.logger.error(f"[{i}/{len(comic_slugs)}] Không tìm thấy thư mục: {slug}")
            failed_count += 1
            continue
        
        if not comic_dir.is_dir():
            themtruyen.logger.error(f"[{i}/{len(comic_slugs)}] {slug} không phải là thư mục")
            failed_count += 1
            continue
        
        themtruyen.logger.info(f"\n[{i}/{len(comic_slugs)}] Xử lý: {slug}")
        
        try:
            if themtruyen.upload_comic_with_chapters(uploader, comic_dir):
                success_count += 1
            else:
                failed_count += 1
        except Exception as e:
            themtruyen.logger.error(f"Lỗi không mong muốn: {str(e)}")
            failed_count += 1
    
    # Tổng kết
    themtruyen.logger.info(f"\n{'='*80}")
    themtruyen.logger.info(f"TEST HOÀN THÀNH!")
    themtruyen.logger.info(f"Đã xử lý: {len(comic_slugs)} truyện")
    themtruyen.logger.info(f"Thành công: {success_count}")
    themtruyen.logger.info(f"Thất bại: {failed_count}")
    themtruyen.logger.info(f"{'='*80}")

if __name__ == '__main__':
    main()
