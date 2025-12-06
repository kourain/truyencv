"""
Script chuẩn bị dữ liệu cho fine-tuning GPT-OSS
Tạo synthetic data từ các truyện convert để training model cải thiện chất lượng dịch
"""

import json
import random
import re
from pathlib import Path
from typing import Dict, List, Tuple
import yaml
from tqdm import tqdm
import argparse


class DataPreparer:
    """Chuẩn bị dữ liệu training từ các truyện convert"""

    def __init__(self, config_path: str):
        with open(config_path, "r", encoding="utf-8") as f:
            self.config = yaml.safe_load(f)

        self.input_dir = Path(self.config["data"].get("input_dir", "./truyen"))
        self.output_dir = Path(self.config["data"].get("output_dir", "./data"))
        self.output_dir.mkdir(parents=True, exist_ok=True)

        self.max_seq_length = self.config["data"]["max_seq_length"]
        self.instruction_template = self.config["instruction"]["instruction_template"]
        self.system_prompt = self.config["instruction"]["system_prompt"]

    def load_novels(self) -> List[Dict]:
        """Đọc tất cả các truyện từ thư mục input"""
        novels = []

        # Lấy tất cả thư mục truyện
        novel_dirs = [d for d in self.input_dir.iterdir() if d.is_dir()]

        print(f"Tìm thấy {len(novel_dirs)} truyện")

        for novel_dir in tqdm(novel_dirs, desc="Đang đọc truyện"):
            # Đọc metadata
            index_file = novel_dir / "index.json"
            if not index_file.exists():
                continue

            with open(index_file, "r", encoding="utf-8") as f:
                metadata = json.load(f)

            # Đọc tất cả các chương
            chapters = []
            chapter_files = sorted(novel_dir.glob("chap-*.txt"))

            for chapter_file in chapter_files:
                with open(chapter_file, "r", encoding="utf-8") as f:
                    content = f.read()
                    chapters.append({"file": chapter_file.name, "content": content})

            novels.append(
                {"metadata": metadata, "chapters": chapters, "dir": novel_dir.name}
            )

        return novels

    def split_into_paragraphs(self, text: str) -> List[str]:
        """Chia văn bản thành các đoạn văn"""
        # Loại bỏ tiêu đề chương
        lines = text.split("\n")
        paragraphs = []
        current_paragraph = []

        for line in lines:
            line = line.strip()

            # Bỏ qua dòng trống và tiêu đề chương
            if not line or line.startswith("Chương") or line.startswith("(Tấu chương"):
                if current_paragraph:
                    paragraphs.append(" ".join(current_paragraph))
                    current_paragraph = []
                continue

            current_paragraph.append(line)

        if current_paragraph:
            paragraphs.append(" ".join(current_paragraph))

        # Lọc các đoạn quá ngắn hoặc quá dài
        filtered_paragraphs = [
            p
            for p in paragraphs
            if 50 < len(p) < 500  # Giới hạn độ dài đoạn văn
        ]

        return filtered_paragraphs

    def degrade_quality(self, text: str) -> str:
        """
        Tạo bản 'kém chất lượng' từ văn bản gốc
        Mô phỏng văn phong máy móc, dịch sát nghĩa
        """
        degraded = text

        # 1. Đơn giản hóa từ vựng - thay thế từ văn học bằng từ thông dụng
        replacements = {
            # Động từ
            "đảo mắt nhìn": "nhìn",
            "ngẩng đầu": "nhìn lên",
            "cúi đầu": "nhìn xuống",
            "lắc đầu": "lắc đầu",
            "gật đầu": "gật đầu",
            "thở dài": "thở ra",
            "trầm ngâm": "suy nghĩ",
            "trầm tư": "suy nghĩ",
            "ngẫm nghĩ": "nghĩ",
            "tự nhủ": "nghĩ thầm",
            "lẩm bẩm": "nói nhỏ",
            "thầm nghĩ": "nghĩ",
            "khẽ nói": "nói nhỏ",
            "cất tiếng": "nói",
            "lên tiếng": "nói",
            # Tính từ
            "khủng khiếp": "rất mạnh",
            "kinh hoàng": "sợ hãi",
            "kinh ngạc": "ngạc nhiên",
            "sửng sốt": "ngạc nhiên",
            "choáng váng": "choáng",
            "mơ hồ": "không rõ",
            "mờ mịt": "mờ",
            "lạ lẫm": "lạ",
            "xa lạ": "lạ",
            "cổ kính": "cổ",
            "huyền bí": "bí ẩn",
            "thần bí": "bí ẩn",
            "hùng vĩ": "to lớn",
            "vĩ đại": "lớn",
            "khổng lồ": "rất lớn",
            "mênh mông": "rất lớn",
            "bao la": "rộng lớn",
            # Danh từ
            "ánh mắt": "mắt",
            "thần sắc": "vẻ mặt",
            "thần thái": "thái độ",
            "khí chất": "phong cách",
            "khí thế": "sức mạnh",
            "khí tức": "hơi thở",
            "linh hồn": "hồn",
            "tâm thần": "tinh thần",
            "ý niệm": "ý nghĩ",
            "tâm niệm": "suy nghĩ",
            # Cụm từ văn học
            "không gì sánh được": "rất",
            "khó tả": "khó nói",
            "vô cùng": "rất",
            "cực kỳ": "rất",
            "vô song": "không ai bằng",
            "tuyệt vời": "rất tốt",
            "phi thường": "không bình thường",
            "bất phàm": "không bình thường",
        }

        for literary, simple in replacements.items():
            degraded = degraded.replace(literary, simple)

        # 2. Loại bỏ các từ trang trí không cần thiết
        degraded = re.sub(
            r"\b(quả thật|thật sự|thực sự|đúng là|quả nhiên)\b", "", degraded
        )

        # 3. Đơn giản hóa cấu trúc câu - loại bỏ dấu phẩy thừa
        degraded = re.sub(r",\s*,", ",", degraded)
        degraded = re.sub(r"\s+", " ", degraded)

        # 4. Thêm một số lỗi dịch máy phổ biến
        # Thay "của hắn" thành "của hắn" (giữ nguyên - đây là văn phong convert)
        # Thay "tại" thành "ở" trong một số trường hợp
        degraded = re.sub(r"\btại\s+(\w+)", r"ở \1", degraded)

        return degraded.strip()

    def create_training_sample(self, original_text: str) -> Dict:
        """Tạo một mẫu training từ đoạn văn gốc"""
        degraded_text = self.degrade_quality(original_text)

        # Format theo chuẩn instruction-response
        sample = {
            "instruction": self.instruction_template,
            "input": degraded_text,
            "output": original_text,
            "system": self.system_prompt,
        }

        return sample

    def prepare_dataset(self) -> Tuple[List[Dict], List[Dict], List[Dict]]:
        """Chuẩn bị toàn bộ dataset"""
        print("Bắt đầu chuẩn bị dataset...")

        # Đọc tất cả truyện
        novels = self.load_novels()

        # Tạo training samples
        all_samples = []

        for novel in tqdm(novels, desc="Xử lý truyện"):
            for chapter in novel["chapters"]:
                paragraphs = self.split_into_paragraphs(chapter["content"])

                for paragraph in paragraphs:
                    sample = self.create_training_sample(paragraph)
                    all_samples.append(sample)

        print(f"Tổng số samples: {len(all_samples)}")

        # Shuffle
        random.shuffle(all_samples)

        # Split dataset
        train_ratio = self.config["split"]["train"]
        val_ratio = self.config["split"]["validation"]

        train_size = int(len(all_samples) * train_ratio)
        val_size = int(len(all_samples) * val_ratio)

        train_data = all_samples[:train_size]
        val_data = all_samples[train_size : train_size + val_size]
        test_data = all_samples[train_size + val_size :]

        print(f"Train: {len(train_data)}, Val: {len(val_data)}, Test: {len(test_data)}")

        return train_data, val_data, test_data

    def save_dataset(
        self, train_data: List[Dict], val_data: List[Dict], test_data: List[Dict]
    ):
        """Lưu dataset ra file JSONL"""
        datasets = {
            "train.jsonl": train_data,
            "val.jsonl": val_data,
            "test.jsonl": test_data,
        }

        for filename, data in datasets.items():
            output_path = self.output_dir / filename
            with open(output_path, "w", encoding="utf-8") as f:
                for sample in data:
                    f.write(json.dumps(sample, ensure_ascii=False) + "\n")
            print(f"Đã lưu {len(data)} samples vào {output_path}")

    def run(self):
        """Chạy toàn bộ pipeline"""
        train_data, val_data, test_data = self.prepare_dataset()
        self.save_dataset(train_data, val_data, test_data)
        print("Hoàn thành chuẩn bị dữ liệu!")


def main():
    parser = argparse.ArgumentParser(
        description="Chuẩn bị dữ liệu cho fine-tuning GPT-OSS"
    )
    parser.add_argument(
        "--config",
        type=str,
        default="configs/training_config.yaml",
        help="Đường dẫn đến file cấu hình",
    )
    parser.add_argument(
        "--input_dir",
        type=str,
        default=None,
        help="Thư mục chứa truyện (override config)",
    )
    parser.add_argument(
        "--output_dir", type=str, default=None, help="Thư mục output (override config)"
    )

    args = parser.parse_args()

    # Load config
    preparer = DataPreparer(args.config)

    # Override nếu có
    if args.input_dir:
        preparer.input_dir = Path(args.input_dir)
    if args.output_dir:
        preparer.output_dir = Path(args.output_dir)
        preparer.output_dir.mkdir(parents=True, exist_ok=True)

    # Run
    preparer.run()


if __name__ == "__main__":
    main()
