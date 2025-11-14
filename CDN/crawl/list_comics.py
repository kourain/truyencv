"""
Script để liệt kê tất cả truyện có sẵn trong thư mục truyen/
"""

import json
from pathlib import Path

TRUYEN_DIR = Path(__file__).parent / 'truyen'

def main():
    """Liệt kê tất cả truyện"""
    
    if not TRUYEN_DIR.exists():
        print(f"❌ Không tìm thấy thư mục {TRUYEN_DIR}")
        print("Vui lòng giải nén truyen.zip hoặc chạy crawler trước")
        return
    
    # Lấy danh sách thư mục
    comic_dirs = [d for d in TRUYEN_DIR.iterdir() if d.is_dir() and d.name != 'ads']
    comic_dirs.sort()
    
    print("="*80)
    print(f"DANH SÁCH TRUYỆN ({len(comic_dirs)} truyện)")
    print("="*80)
    print()
    
    # Hiển thị thông tin từng truyện
    for i, comic_dir in enumerate(comic_dirs, 1):
        slug = comic_dir.name
        index_file = comic_dir / 'index.json'
        
        # Đọc thông tin từ index.json nếu có
        if index_file.exists():
            try:
                with open(index_file, 'r', encoding='utf-8') as f:
                    data = json.load(f)
                
                title = data.get('title', slug)
                author = data.get('author', 'N/A')
                status = data.get('comic_status', 'N/A')
            except:
                title = slug
                author = 'N/A'
                status = 'N/A'
        else:
            title = slug
            author = 'N/A'
            status = 'N/A'
        
        # Đếm số chương
        chapters = [f for f in comic_dir.iterdir() if f.is_file() and f.name.startswith('chap-')]
        chapter_count = len(chapters)
        
        # In thông tin
        print(f"{i:3}. {title}")
        print(f"     Slug: {slug}")
        print(f"     Tác giả: {author}")
        print(f"     Trạng thái: {status}")
        print(f"     Số chương: {chapter_count}")
        print()
    
    print("="*80)
    print(f"Tổng cộng: {len(comic_dirs)} truyện")
    print("="*80)
    print()
    print("Để test upload một vài truyện, sử dụng:")
    print("  python themtruyen_test.py <slug1> <slug2> ...")
    print()
    print("Ví dụ:")
    if len(comic_dirs) > 0:
        print(f"  python themtruyen_test.py {comic_dirs[0].name}")
    if len(comic_dirs) > 1:
        print(f"  python themtruyen_test.py {comic_dirs[0].name} {comic_dirs[1].name}")

if __name__ == '__main__':
    main()
