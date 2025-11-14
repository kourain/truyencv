"""
Script để tự động thêm truyện và chương vào backend API từ thư mục truyen/

Yêu cầu:
- Python 3.8+
- requests library: pip install requests
- Thiết lập biến môi trường API_BASE_URL và API_TOKEN
"""

import os
import json
import time
import logging
from pathlib import Path
from typing import Optional, Dict, List, Any
import requests
from requests.adapters import HTTPAdapter
from urllib3.util.retry import Retry

# Cấu hình logging
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(levelname)s - %(message)s',
    handlers=[
        logging.FileHandler('themtruyen.log', encoding='utf-8'),
        logging.StreamHandler()
    ]
)
logger = logging.getLogger(__name__)

# Cấu hình
API_BASE_URL = os.getenv('API_BASE_URL', 'http://localhost:44344')
API_TOKEN = os.getenv('API_TOKEN', '')
TRUYEN_DIR = Path(__file__).parent / 'truyen'
DELAY_BETWEEN_REQUESTS = float(os.getenv('DELAY_BETWEEN_REQUESTS', '0.5'))  # seconds
MAX_RETRIES = int(os.getenv('MAX_RETRIES', '3'))

# Mapping trạng thái truyện
COMIC_STATUS_MAPPING = {
    'Còn tiếp': 1,  # Continuing
    'Tạm dừng': 2,  # Paused
    'Đã dừng': 3,   # Stopped
    'Hoàn thành': 4,  # Completed
    'Bị cấm': 5     # Banned
}


class ComicUploader:
    """Class xử lý upload truyện và chương lên API"""
    
    def __init__(self, base_url: str, token: str):
        self.base_url = base_url.rstrip('/')
        self.token = token
        self.session = self._create_session()
        
    def _create_session(self) -> requests.Session:
        """Tạo session với retry strategy"""
        session = requests.Session()
        
        # Cấu hình retry cho các lỗi tạm thời
        retry_strategy = Retry(
            total=MAX_RETRIES,
            backoff_factor=1,
            status_forcelist=[429, 500, 502, 503, 504],
            allowed_methods=["HEAD", "GET", "OPTIONS", "POST", "PUT"]
        )
        
        adapter = HTTPAdapter(max_retries=retry_strategy)
        session.mount("http://", adapter)
        session.mount("https://", adapter)
        
        # Set headers
        session.headers.update({
            'Authorization': f'Bearer {self.token}',
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        })
        
        return session
    
    def _get_comic_status(self, status_text: str) -> int:
        """Chuyển đổi trạng thái text sang enum"""
        return COMIC_STATUS_MAPPING.get(status_text, 1)  # Default: Continuing
    
    def create_comic(self, comic_data: Dict[str, Any]) -> Optional[Dict[str, Any]]:
        """
        Tạo truyện mới qua API
        
        Args:
            comic_data: Dictionary chứa thông tin truyện từ index.json
            
        Returns:
            Response từ API hoặc None nếu thất bại
        """
        url = f"{self.base_url}/Converter/Comic"
        
        # Chuẩn bị payload theo format API
        payload = {
            "name": comic_data.get("title", ""),
            "description": comic_data.get("description", ""),
            "author": comic_data.get("author", ""),
            "embedded_from": comic_data.get("embedded_from"),
            "embedded_from_url": comic_data.get("embedded_from_url"),
            "cover_url": comic_data.get("cover_url"),
            "main_category_id": comic_data.get("main_category_id", 1001),
            "category_ids": comic_data.get("category_ids", []),
            "status": self._get_comic_status(comic_data.get("comic_status", "Còn tiếp"))
        }
        
        try:
            logger.info(f"Đang tạo truyện: {payload['name']}")
            response = self.session.post(url, json=payload, timeout=30)
            
            if response.status_code == 201:
                result = response.json()
                logger.info(f"✓ Tạo truyện thành công: {payload['name']} (ID: {result.get('id')})")
                return result
            elif response.status_code == 400:
                error_msg = response.json().get('message', 'Unknown error')
                logger.error(f"✗ Lỗi tạo truyện {payload['name']}: {error_msg}")
                return None
            elif response.status_code == 401:
                logger.error(f"✗ Lỗi xác thực - Token không hợp lệ hoặc thiếu quyền Converter")
                return None
            else:
                logger.error(f"✗ Lỗi tạo truyện {payload['name']}: HTTP {response.status_code}")
                logger.error(f"Response: {response.text}")
                return None
                
        except requests.exceptions.RequestException as e:
            logger.error(f"✗ Lỗi kết nối khi tạo truyện {payload['name']}: {str(e)}")
            return None
    
    def create_chapter(self, comic_id: str, chapter_num: int, content: str) -> Optional[Dict[str, Any]]:
        """
        Tạo chương mới qua API
        
        Args:
            comic_id: ID của truyện (string format)
            chapter_num: Số thứ tự chương
            content: Nội dung chương
            
        Returns:
            Response từ API hoặc None nếu thất bại
        """
        url = f"{self.base_url}/Converter/ComicChapter"
        
        payload = {
            "comic_id": str(comic_id),
            "chapter": chapter_num,
            "content": content,
            "key_require": None,
            "key_require_until": None
        }
        
        try:
            logger.info(f"  Đang tạo chương {chapter_num} (ID truyện: {comic_id})")
            response = self.session.post(url, json=payload, timeout=30)
            
            if response.status_code == 201:
                result = response.json()
                logger.info(f"  ✓ Tạo chương {chapter_num} thành công")
                return result
            elif response.status_code == 400:
                error_msg = response.json().get('message', 'Unknown error')
                logger.error(f"  ✗ Lỗi tạo chương {chapter_num}: {error_msg}")
                return None
            elif response.status_code == 401:
                logger.error(f"  ✗ Lỗi xác thực khi tạo chương {chapter_num}")
                return None
            elif response.status_code == 404:
                logger.error(f"  ✗ Không tìm thấy truyện với ID {comic_id}")
                return None
            else:
                logger.error(f"  ✗ Lỗi tạo chương {chapter_num}: HTTP {response.status_code}")
                logger.error(f"  Response: {response.text}")
                return None
                
        except requests.exceptions.RequestException as e:
            logger.error(f"  ✗ Lỗi kết nối khi tạo chương {chapter_num}: {str(e)}")
            return None


def read_chapter_content(chapter_file: Path) -> str:
    """Đọc nội dung chapter từ file"""
    try:
        with open(chapter_file, 'r', encoding='utf-8') as f:
            return f.read()
    except Exception as e:
        logger.error(f"Lỗi đọc file {chapter_file}: {str(e)}")
        return ""


def get_chapter_files(comic_dir: Path) -> List[tuple]:
    """
    Lấy danh sách các file chapter và số thứ tự
    
    Returns:
        List của tuple (chapter_number, file_path)
    """
    chapters = []
    
    for file in comic_dir.iterdir():
        if file.is_file() and file.name.startswith('chap-'):
            try:
                # Extract chapter number from filename
                chapter_num_str = file.name.replace('chap-', '').replace('.txt', '')
                chapter_num = int(chapter_num_str)
                chapters.append((chapter_num, file))
            except ValueError:
                logger.warning(f"Không thể parse số chương từ file: {file.name}")
                continue
    
    # Sắp xếp theo số thứ tự chương
    chapters.sort(key=lambda x: x[0])
    return chapters


def upload_comic_with_chapters(uploader: ComicUploader, comic_dir: Path) -> bool:
    """
    Upload một truyện với tất cả các chương
    
    Args:
        uploader: Instance của ComicUploader
        comic_dir: Thư mục chứa truyện
        
    Returns:
        True nếu thành công, False nếu thất bại
    """
    slug = comic_dir.name
    index_file = comic_dir / 'index.json'
    
    # Kiểm tra file index.json
    if not index_file.exists():
        logger.warning(f"Bỏ qua {slug}: Không tìm thấy index.json")
        return False
    
    # Đọc thông tin truyện
    try:
        with open(index_file, 'r', encoding='utf-8') as f:
            comic_data = json.load(f)
    except Exception as e:
        logger.error(f"Lỗi đọc index.json của {slug}: {str(e)}")
        return False
    
    logger.info(f"\n{'='*80}")
    logger.info(f"Bắt đầu xử lý truyện: {comic_data.get('title', slug)}")
    logger.info(f"{'='*80}")
    
    # Tạo truyện
    comic_result = uploader.create_comic(comic_data)
    if not comic_result:
        logger.error(f"Không thể tạo truyện {slug}, bỏ qua.")
        return False
    
    time.sleep(DELAY_BETWEEN_REQUESTS)
    
    comic_id = comic_result.get('id')
    if not comic_id:
        logger.error(f"Không nhận được ID của truyện {slug}")
        return False
    
    # Lấy danh sách chapters
    chapters = get_chapter_files(comic_dir)
    if not chapters:
        logger.warning(f"Không tìm thấy chương nào cho truyện {slug}")
        return True  # Vẫn coi là thành công vì đã tạo truyện
    
    logger.info(f"Tìm thấy {len(chapters)} chương, bắt đầu upload...")
    
    # Upload từng chương
    success_count = 0
    failed_count = 0
    
    for chapter_num, chapter_file in chapters:
        content = read_chapter_content(chapter_file)
        if not content:
            logger.warning(f"  Bỏ qua chương {chapter_num}: Nội dung rỗng")
            failed_count += 1
            continue
        
        result = uploader.create_chapter(comic_id, chapter_num, content)
        if result:
            success_count += 1
        else:
            failed_count += 1
        
        # Delay giữa các request
        time.sleep(DELAY_BETWEEN_REQUESTS)
    
    logger.info(f"\nKết quả: {success_count} chương thành công, {failed_count} chương thất bại")
    return True


def main():
    """Hàm chính"""
    
    # Kiểm tra cấu hình
    if not API_TOKEN:
        logger.error("Lỗi: Chưa thiết lập API_TOKEN")
        logger.error("Vui lòng thiết lập biến môi trường API_TOKEN")
        logger.error("Ví dụ: export API_TOKEN='your-jwt-token-here'")
        return
    
    if not TRUYEN_DIR.exists():
        logger.error(f"Lỗi: Không tìm thấy thư mục {TRUYEN_DIR}")
        logger.error("Vui lòng chạy script trong thư mục CDN/crawl/")
        return
    
    logger.info(f"Bắt đầu upload truyện từ thư mục: {TRUYEN_DIR}")
    logger.info(f"API Base URL: {API_BASE_URL}")
    logger.info(f"Delay giữa các request: {DELAY_BETWEEN_REQUESTS}s")
    logger.info("="*80)
    
    # Tạo uploader instance
    uploader = ComicUploader(API_BASE_URL, API_TOKEN)
    
    # Lấy danh sách các thư mục truyện
    comic_dirs = [d for d in TRUYEN_DIR.iterdir() if d.is_dir() and d.name != 'ads']
    logger.info(f"Tìm thấy {len(comic_dirs)} truyện\n")
    
    # Upload từng truyện
    total_success = 0
    total_failed = 0
    
    for i, comic_dir in enumerate(comic_dirs, 1):
        logger.info(f"\n[{i}/{len(comic_dirs)}] Xử lý truyện: {comic_dir.name}")
        
        try:
            if upload_comic_with_chapters(uploader, comic_dir):
                total_success += 1
            else:
                total_failed += 1
        except Exception as e:
            logger.error(f"Lỗi không mong muốn khi xử lý {comic_dir.name}: {str(e)}")
            total_failed += 1
        
        # Thêm delay giữa các truyện
        if i < len(comic_dirs):
            time.sleep(DELAY_BETWEEN_REQUESTS * 2)
    
    # Tổng kết
    logger.info(f"\n{'='*80}")
    logger.info(f"HOÀN THÀNH!")
    logger.info(f"Tổng số truyện: {len(comic_dirs)}")
    logger.info(f"Thành công: {total_success}")
    logger.info(f"Thất bại: {total_failed}")
    logger.info(f"{'='*80}")


if __name__ == '__main__':
    main()
