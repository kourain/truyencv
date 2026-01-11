#!/usr/bin/env python3
"""
Script tải và xử lý dataset ngochuyen_voice từ Hugging Face cho XTTS fine-tuning.

Dataset: pnnbao-ump/ngochuyen_voice
Số mẫu: ~7,540 audio files
Kích thước: ~3.27GB

Output:
- data/ngochuyen_voice/wavs/*.wav (audio files 24kHz mono)
- data/ngochuyen_voice/metadata_train.csv (95% dataset)
- data/ngochuyen_voice/metadata_val.csv (5% dataset)
"""

import csv
import os
from pathlib import Path
from typing import Tuple
import argparse

import soundfile as sf
import librosa
import numpy as np
from datasets import load_dataset
from tqdm import tqdm
from underthesea import sent_tokenize, text_normalize
from TTS.vietnamese.text_processor import process_vietnamese_text
def _normalize_text(text: str) -> str:
    text = text.replace("·", "")
    print(".",end="")
    # open("temp.txt", "w", encoding="utf-8").write(text)
    cleaned = (
        str(text_normalize(text))
        .replace("..", ".")
        .replace("!.", "!")
        .replace("?.", "?")
        .replace(" .", ".")
        .replace(" ,", ",")
        .replace('"', "")
        .replace("'", "")
        .replace("AI", "Ây Ai")
        .replace("A.I", "Ây Ai")
        .replace("năm 1.", "năm nhất.")
        .replace("năm 2.", "năm hai.")
        .replace("năm 3.", "năm ba.")
        .replace("năm 4.", "năm bốn.")
        .replace("năm 5.", "năm năm.")
        .replace("năm 6.", "năm sáu.")
        .replace("năm 7.", "năm bảy.")
        .replace("năm 8.", "năm tám.")
        .replace("năm 9.", "năm chín.")
        .replace("năm 10.", "năm mười.")
    )
    return process_vietnamese_text(cleaned)

def setup_directories(output_dir: Path) -> Tuple[Path, Path]:
    """Tạo cấu trúc thư mục cho dataset."""
    wavs_dir = output_dir / "wavs"
    wavs_dir.mkdir(parents=True, exist_ok=True)
    return output_dir, wavs_dir


def process_audio(audio_array: np.ndarray, sample_rate: int, target_sr: int = 24000) -> np.ndarray:
    """Xử lý audio: chuyển sang mono và resample về 24kHz."""
    # Chuyển sang mono nếu stereo
    if len(audio_array.shape) > 1:
        audio_array = librosa.to_mono(audio_array.T)
    
    # Resample về 24kHz nếu cần
    if sample_rate != target_sr:
        audio_array = librosa.resample(audio_array, orig_sr=sample_rate, target_sr=target_sr)
    
    return audio_array


def download_and_process_dataset(
    dataset_name: str,
    output_dir: Path,
    train_split: float = 0.95,
    preview_only: bool = False,
    max_samples: int = None,
) -> Tuple[int, int]:
    """
    Tải và xử lý dataset từ Hugging Face.
    
    Args:
        dataset_name: Tên dataset trên Hugging Face
        output_dir: Thư mục output
        train_split: Tỷ lệ chia train/val (default: 0.95)
        preview_only: Chỉ tải vài mẫu để xem trước
        max_samples: Số lượng mẫu tối đa (None = tải hết)
    
    Returns:
        Tuple[số mẫu train, số mẫu val]
    """
    print(f"Đang tải dataset {dataset_name} từ Hugging Face...")
    
    # Tải dataset
    dataset = load_dataset(dataset_name, split="train")
    
    if preview_only:
        dataset = dataset.select(range(min(10, len(dataset))))
        print(f"[Preview mode] Chỉ xử lý {len(dataset)} mẫu đầu tiên")
    elif max_samples:
        dataset = dataset.select(range(min(max_samples, len(dataset))))
        print(f"Giới hạn {len(dataset)} mẫu")
    
    print(f"Tổng số mẫu: {len(dataset)}")
    
    # Tạo thư mục
    _, wavs_dir = setup_directories(output_dir)
    
    # Danh sách lưu metadata
    train_rows = []
    val_rows = []
    
    # Tính số lượng train/val
    total_samples = len(dataset)
    train_count = int(total_samples * train_split)
    
    print(f"Đang xử lý {total_samples} mẫu audio...")
    print(f"Train: {train_count} mẫu ({train_split*100:.1f}%)")
    print(f"Val: {total_samples - train_count} mẫu ({(1-train_split)*100:.1f}%)")
    
    # Xử lý từng mẫu
    for idx, sample in enumerate(tqdm(dataset, desc="Processing")):
        try:
            # Lấy thông tin audio và text
            audio_data = sample.get("audio")
            text:str = sample.get("transcription", "")
            if not audio_data or not text:
                print(f"Bỏ qua mẫu {idx}: thiếu audio hoặc text")
                break
            text = text.replace('\n', ' ').replace('\r', ' ').replace("|","").strip()
            text = _normalize_text(text)
            # Xử lý audio
            audio_array = np.array(audio_data["array"])
            sample_rate = audio_data["sampling_rate"]
            
            # Chuyển đổi audio sang 24kHz mono
            processed_audio = process_audio(audio_array, sample_rate, target_sr=24000)
            
            # Tạo tên file
            filename = f"audio_{idx:05d}.wav"
            wav_path = wavs_dir / filename
            
            # Lưu audio
            sf.write(wav_path, processed_audio, 24000)
            
            # Lấy speaker name (nếu có, mặc định là "speaker_001")
            speaker_name = sample.get("speaker", "speaker_001")
            
            # Thêm vào train hoặc val
            row = [f"wavs/{filename}", text, speaker_name]
            if idx < train_count:
                train_rows.append(row)
            else:
                val_rows.append(row)
                
        except Exception as e:
            print(f"Lỗi xử lý mẫu {idx}: {e}")
            continue
    
    # Lưu manifest files
    train_manifest = output_dir / "metadata_train.csv"
    val_manifest = output_dir / "metadata_val.csv"
    
    # Ghi train manifest
    with train_manifest.open("w", encoding="utf-8", newline="") as f:
        writer = csv.writer(f, delimiter="|")
        writer.writerow(["audio_file", "text", "speaker_name"])
        writer.writerows(train_rows)
    
    # Ghi val manifest
    with val_manifest.open("w", encoding="utf-8", newline="") as f:
        writer = csv.writer(f, delimiter="|")
        writer.writerow(["audio_file", "text", "speaker_name"])
        writer.writerows(val_rows)
    
    print(f"\n✓ Hoàn tất!")
    print(f"  Train manifest: {train_manifest} ({len(train_rows)} mẫu)")
    print(f"  Val manifest: {val_manifest} ({len(val_rows)} mẫu)")
    print(f"  Audio files: {wavs_dir}")
    
    return len(train_rows), len(val_rows)


def main():
    parser = argparse.ArgumentParser(
        description="Tải và xử lý dataset ngochuyen_voice từ Hugging Face"
    )
    parser.add_argument(
        "--output-dir",
        type=str,
        default="data/ngochuyen_voice",
        help="Thư mục output (default: data/ngochuyen_voice)",
    )
    parser.add_argument(
        "--dataset-name",
        type=str,
        default="pnnbao-ump/ngochuyen_voice",
        help="Tên dataset trên Hugging Face (default: pnnbao-ump/ngochuyen_voice)",
    )
    parser.add_argument(
        "--train-split",
        type=float,
        default=0.95,
        help="Tỷ lệ train/val (default: 0.95)",
    )
    parser.add_argument(
        "--preview",
        action="store_true",
        help="Chỉ tải 10 mẫu đầu để xem trước",
    )
    parser.add_argument(
        "--max-samples",
        type=int,
        default=None,
        help="Số lượng mẫu tối đa (default: None = tải hết)",
    )
    
    args = parser.parse_args()
    
    output_dir = Path(args.output_dir).resolve()
    
    print("=" * 60)
    print("XTTS Dataset Preparation - ngochuyen_voice")
    print("=" * 60)
    print(f"Dataset: {args.dataset_name}")
    print(f"Output: {output_dir}")
    print(f"Train/Val split: {args.train_split*100:.0f}/{(1-args.train_split)*100:.0f}")
    print("=" * 60)
    
    train_count, val_count = download_and_process_dataset(
        dataset_name=args.dataset_name,
        output_dir=output_dir,
        train_split=args.train_split,
        preview_only=args.preview,
        max_samples=args.max_samples,
    )
    
    print("\n" + "=" * 60)
    print("Tổng kết:")
    print(f"  ✓ Train: {train_count} mẫu")
    print(f"  ✓ Val: {val_count} mẫu")
    print(f"  ✓ Tổng: {train_count + val_count} mẫu")
    print("=" * 60)
    print("\nBước tiếp theo:")
    print("1. Kiểm tra manifest files trong:", output_dir)
    print("2. Cập nhật config.json với đường dẫn dataset")
    print("3. Chạy training với notebook fine_tune_ngochuyen.ipynb")


if __name__ == "__main__":
    main()
