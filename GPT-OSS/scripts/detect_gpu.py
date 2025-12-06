"""
Script tự động detect GPU và đề xuất config phù hợp
"""

import torch
import subprocess


def get_gpu_info():
    """Lấy thông tin GPU"""
    if not torch.cuda.is_available():
        return None, 0

    gpu_name = torch.cuda.get_device_name(0)
    gpu_memory = torch.cuda.get_device_properties(0).total_memory / (1024**3)  # GB

    return gpu_name, gpu_memory


def recommend_config(gpu_name, gpu_memory):
    """Đề xuất config dựa trên GPU"""
    gpu_name_lower = gpu_name.lower()

    configs = {
        "rtx 3050": {
            "config": "training_config.yaml",
            "batch_size": 1,
            "grad_accum": 16,
            "max_seq_len": 1024,
        },
        "t4": {
            "config": "training_config_t4.yaml",
            "batch_size": 2,
            "grad_accum": 8,
            "max_seq_len": 2048,
        },
        "v100": {
            "config": "training_config_v100.yaml",
            "batch_size": 4,
            "grad_accum": 4,
            "max_seq_len": 2048,
        },
        "a100": {
            "config": "training_config_a100.yaml",
            "batch_size": 8,
            "grad_accum": 2,
            "max_seq_len": 4096,
        },
        "rtx 4090": {
            "config": "training_config_4090.yaml",
            "batch_size": 4,
            "grad_accum": 4,
            "max_seq_len": 2048,
        },
    }

    # Tìm config phù hợp
    for key, config in configs.items():
        if key in gpu_name_lower:
            return config

    # Fallback dựa trên VRAM
    if gpu_memory < 6:
        return configs["rtx 3050"]
    elif gpu_memory < 12:
        return configs["rtx 3050"]
    elif gpu_memory < 18:
        return configs["t4"]
    elif gpu_memory < 30:
        return configs["v100"]
    else:
        return configs["a100"]


def main():
    print("=" * 60)
    print("GPU DETECTION & CONFIG RECOMMENDATION")
    print("=" * 60)
    print()

    # Check CUDA
    if not torch.cuda.is_available():
        print(" CUDA không khả dụng!")
        print("   Training sẽ rất chậm trên CPU.")
        print("   Khuyến nghị: Sử dụng Google Colab với GPU miễn phí")
        print()
        print("   Link: https://colab.research.google.com/")
        return

    # Get GPU info
    gpu_name, gpu_memory = get_gpu_info()

    print(" GPU được phát hiện:")
    print(f"   Tên: {gpu_name}")
    print(f"   VRAM: {gpu_memory:.2f} GB")
    print()

    # Get recommendation
    config = recommend_config(gpu_name, gpu_memory)

    print(" Cấu hình được đề xuất:")
    print(f"   Config file: configs/{config['config']}")
    print(f"   Batch size: {config['batch_size']}")
    print(f"   Gradient accumulation: {config['grad_accum']}")
    print(f"   Max sequence length: {config['max_seq_len']}")
    print()

    # Command to run
    print(" Lệnh để chạy training:")
    print(f"   python scripts/train.py --config configs/{config['config']}")
    print()

    # Additional tips
    if (
        "t4" in gpu_name.lower()
        or "colab" in str(subprocess.getenv("COLAB_GPU", "")).lower()
    ):
        print(" Tips cho Google Colab:")
        print("   - Colab Free có giới hạn 12h/session")
        print("   - Nhớ save checkpoints thường xuyên")
        print("   - Có thể resume training từ checkpoint")
        print("   - Xem thêm: colab_training.ipynb")
        print()

    if gpu_memory < 6:
        print(" Cảnh báo:")
        print("   VRAM thấp, có thể gặp Out of Memory")
        print("   Nếu gặp lỗi, hãy:")
        print("   - Giảm max_seq_length xuống 512")
        print("   - Tăng gradient_accumulation_steps lên 32")
        print()


if __name__ == "__main__":
    main()
