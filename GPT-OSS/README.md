# GPT-OSS Fine-tuning cho Cải thiện Dịch Truyện Convert

Dự án này sử dụng GPT-OSS 20B để cải thiện chất lượng bản dịch truyện convert từ tiếng Trung sang tiếng Việt, biến đổi văn phong máy móc thành văn học tự nhiên hơn.

## 🎯 Mục tiêu

- **Input**: Bản dịch convert kém chất lượng (máy móc, sát nghĩa)
- **Output**: Bản dịch văn học, tự nhiên, giữ nguyên phong cách tiểu thuyết
- **Model**: GPT-OSS 20B với QLoRA fine-tuning
- **Hardware**: RTX 3050 Laptop (4GB VRAM + 16GB Shared)

## 📁 Cấu trúc Dự án

```
GPT-OSS/
├── truyen/                    # Dữ liệu truyện gốc (203 truyện)
│   ├── [ten-truyen]/
│   │   ├── index.json        # Metadata truyện
│   │   └── chap-*.txt        # Nội dung các chương
├── scripts/                   # Scripts xử lý và training
│   ├── prepare_data.py       # Chuẩn bị dataset
│   ├── train.py              # Fine-tuning script
│   ├── inference.py          # Test model
│   └── evaluate.py           # Đánh giá chất lượng
├── data/                      # Dataset đã xử lý
│   ├── train.jsonl
│   ├── val.jsonl
│   └── test.jsonl
├── models/                    # Model checkpoints
│   └── lora_adapters/
├── outputs/                   # Kết quả dịch
└── configs/                   # Cấu hình training
    └── training_config.yaml
```

## 🔧 Yêu cầu Hệ thống

### Phần cứng
- GPU: RTX 3050 Laptop (4GB VRAM) hoặc tương đương
- RAM: 16GB+
- Disk: 50GB+ (cho model và data)

### Phần mềm
- Python 3.11
- CUDA 12.8+
- PyTorch 2.9.1+

## 📦 Cài đặt

### 1. Cài đặt Dependencies

```bash
# Sử dụng uv (đã có trong pyproject.toml)
uv pip install -e .
```

### 2. Cài đặt thêm các thư viện cho fine-tuning

```bash
uv pip install bitsandbytes accelerate peft trl datasets
```

## 🎮 Multi-GPU Support

Dự án hỗ trợ nhiều loại GPU với configs tối ưu riêng:

| GPU | VRAM | Config File | Batch Size | Training Time |
|-----|------|-------------|------------|---------------|
| RTX 3050 Laptop | 4GB | `training_config.yaml` | 1 | ~3 days |
| **Google Colab T4** | 15GB | `training_config_t4.yaml` | 2 | ~1.5 days |
| Google Colab V100 | 16GB | `training_config_v100.yaml` | 4 | ~1 day |
| Google Colab A100 | 40GB | `training_config_a100.yaml` | 8 | ~15 hours |
| RTX 4090 | 24GB | `training_config_4090.yaml` | 4 | ~18 hours |

### Auto-detect GPU

```bash
python scripts/detect_gpu.py
```

### Google Colab (Khuyến nghị!)

**Ưu điểm:**
- ✅ GPU miễn phí (T4 15GB)
- ✅ Không cần cài đặt local
- ✅ Training nhanh hơn 2x so với RTX 3050
- ✅ Có thể upgrade lên V100/A100 với Colab Pro

**Cách sử dụng:**
1. Upload project lên Google Drive
2. Mở `colab_training.ipynb` trong Colab
3. Chạy từng cell theo hướng dẫn

**Lưu ý:** Colab Free có giới hạn 12h/session, nhưng có thể resume từ checkpoint.

Xem chi tiết: [configs/GPU_CONFIGS.md](configs/GPU_CONFIGS.md)

## 🚀 Quy trình Fine-tuning

### Bước 1: Chuẩn bị Dữ liệu

```bash
python scripts/prepare_data.py --input_dir truyen/ --output_dir data/
```

Script này sẽ:
- Đọc tất cả các truyện từ thư mục `truyen/`
- Tạo synthetic data bằng cách sử dụng GPT-OSS để tạo các phiên bản "kém chất lượng" từ văn bản gốc
- Chia dataset: 80% train, 10% validation, 10% test
- Format theo chuẩn instruction-response

### Bước 2: Fine-tuning với QLoRA

```bash
python scripts/train.py --config configs/training_config.yaml
```

Cấu hình training:
- **QLoRA**: 4-bit quantization để giảm VRAM
- **LoRA rank**: 16
- **Batch size**: 1 (với gradient accumulation)
- **Learning rate**: 2e-4
- **Epochs**: 3-5

### Bước 3: Inference và Test

```bash
python scripts/inference.py --model_path models/lora_adapters/ --input "văn bản cần cải thiện"
```

### Bước 4: Đánh giá

```bash
python scripts/evaluate.py --test_file data/test.jsonl
```

## 📊 Chiến lược Training

### 1. Synthetic Data Generation
Vì không có parallel corpus (bản kém vs bản tốt), chúng ta sẽ:
- Sử dụng bản dịch hiện có làm "ground truth" (bản tốt)
- Tạo bản "kém chất lượng" bằng cách:
  - Đơn giản hóa từ vựng
  - Dịch sát nghĩa hơn
  - Loại bỏ các yếu tố văn học

### 2. Instruction Format
```json
{
  "instruction": "Cải thiện chất lượng bản dịch sau đây, làm cho nó tự nhiên và văn học hơn:",
  "input": "[Bản dịch kém chất lượng]",
  "output": "[Bản dịch chất lượng cao]"
}
```

### 3. Optimization cho 4GB VRAM
- **QLoRA**: 4-bit quantization
- **Gradient Checkpointing**: Giảm memory usage
- **Gradient Accumulation**: Tăng effective batch size
- **Mixed Precision**: BF16 training

## 🎨 Ví dụ Cải thiện

**Input (Kém chất lượng):**
```
Tần Vũ nhìn bốn phía, cổ kính hoàn cảnh, lạ lẫm không gì sánh được.
```

**Output (Văn học):**
```
Tần Vũ đảo mắt nhìn quanh, khung cảnh cổ kính hiện ra trước mắt, xa lạ đến mức khó tả.
```

## 📈 Monitoring

Training metrics được log qua:
- TensorBoard
- Weights & Biases (optional)
- Console output

## 🔍 Troubleshooting

### Out of Memory (OOM)
- Giảm `per_device_train_batch_size` xuống 1
- Tăng `gradient_accumulation_steps`
- Giảm `max_seq_length`

### Slow Training
- Sử dụng `torch.compile()` (PyTorch 2.0+)
- Enable `flash_attention_2`
- Giảm `lora_rank`

## 📝 Notes

- Model GPT-OSS 20B sẽ được tự động tải từ Hugging Face
- LoRA adapters nhẹ (~100MB) so với full model (~40GB)
- Training time: ~2-3 ngày trên RTX 3050 Laptop

## 🤝 Contributing

Mọi đóng góp đều được hoan nghênh! Vui lòng tạo issue hoặc pull request.

## 📄 License

MIT License
