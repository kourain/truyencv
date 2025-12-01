# Hướng dẫn Sử dụng GPT-OSS Fine-tuning

## Bước 1: Cài đặt Dependencies

```bash
cd E:\Code\DATN\GPT-OSS

# Cài đặt tất cả dependencies
uv pip install -e .
```

**Lưu ý**: Quá trình cài đặt có thể mất 10-20 phút do cần tải model và các thư viện lớn.

## Bước 2: Chuẩn bị Dữ liệu

```bash
# Chạy script chuẩn bị dữ liệu
python scripts/prepare_data.py --input_dir truyen/ --output_dir data/
```

Script này sẽ:
- Đọc tất cả 203 truyện từ thư mục `truyen/`
- Tạo synthetic data (bản kém chất lượng vs bản tốt)
- Chia dataset: 80% train, 10% val, 10% test
- Lưu vào thư mục `data/`

**Thời gian ước tính**: 30-60 phút tùy thuộc vào số lượng truyện

## Bước 3: Fine-tuning Model

```bash
# Chạy training
python scripts/train.py --config configs/training_config.yaml
```

**Cấu hình cho RTX 3050 4GB:**
- QLoRA 4-bit quantization
- Batch size: 1
- Gradient accumulation: 16 steps
- Max sequence length: 1024 tokens

**Thời gian ước tính**: 2-3 ngày (có thể dừng và resume sau)

### Resume Training từ Checkpoint

Nếu training bị gián đoạn:

```bash
python scripts/train.py --config configs/training_config.yaml --resume_from_checkpoint models/lora_adapters/checkpoint-XXX
```

## Bước 4: Test Model

### Chế độ Interactive

```bash
python scripts/inference.py --model_path models/lora_adapters/ --interactive
```

Sau đó nhập văn bản cần cải thiện.

### Test với File

```bash
python scripts/inference.py --model_path models/lora_adapters/ --input_file input.txt --output_file output.txt
```

### Test với Text trực tiếp

```bash
python scripts/inference.py --model_path models/lora_adapters/ --input "Tần Vũ nhìn bốn phía, cổ kính hoàn cảnh, lạ lẫm không gì sánh được."
```

## Bước 5: Đánh giá Model

```bash
# Đánh giá trên toàn bộ test set
python scripts/evaluate.py --model_path models/lora_adapters/ --test_file data/test.jsonl --output_file outputs/evaluation_results.json

# Đánh giá trên 100 samples đầu tiên (nhanh hơn)
python scripts/evaluate.py --model_path models/lora_adapters/ --test_file data/test.jsonl --num_samples 100
```

## Monitoring Training

### TensorBoard

```bash
# Mở TensorBoard để xem training progress
tensorboard --logdir outputs/logs
```

Sau đó mở browser tại: http://localhost:6006

## Troubleshooting

### Out of Memory (OOM)

Nếu gặp lỗi OOM, giảm các tham số sau trong `configs/training_config.yaml`:

```yaml
data:
  max_seq_length: 512  # Giảm từ 1024

training:
  gradient_accumulation_steps: 32  # Tăng từ 16
```

### Training quá chậm

- Đảm bảo CUDA được cài đặt đúng
- Kiểm tra GPU đang được sử dụng: `nvidia-smi`
- Giảm `lora.r` từ 16 xuống 8

### Model không cải thiện

- Tăng số epochs từ 3 lên 5
- Điều chỉnh learning rate
- Kiểm tra chất lượng dữ liệu training

## Tips & Tricks

### 1. Tối ưu hóa VRAM

- Đóng tất cả ứng dụng khác khi training
- Sử dụng `max_seq_length` nhỏ nhất có thể
- Enable gradient checkpointing (đã bật mặc định)

### 2. Cải thiện Chất lượng

- Thêm nhiều ví dụ training hơn
- Fine-tune thêm trên các đoạn văn đặc biệt
- Điều chỉnh temperature khi inference (0.5-0.9)

### 3. Tăng Tốc Training

- Sử dụng `torch.compile()` (PyTorch 2.0+)
- Giảm `lora_rank` nếu có thể
- Sử dụng mixed precision training (đã bật bf16)

## Ví dụ Workflow Hoàn chỉnh

```bash
# 1. Cài đặt
uv pip install -e .

# 2. Chuẩn bị data
python scripts/prepare_data.py

# 3. Training (để chạy qua đêm)
python scripts/train.py

# 4. Test nhanh
python scripts/inference.py --model_path models/lora_adapters/ --interactive

# 5. Đánh giá
python scripts/evaluate.py --model_path models/lora_adapters/ --num_samples 100
```

## Cấu trúc Output

```
GPT-OSS/
├── data/
│   ├── train.jsonl          # Training data
│   ├── val.jsonl            # Validation data
│   └── test.jsonl           # Test data
├── models/
│   ├── cache/               # Model cache
│   └── lora_adapters/       # Fine-tuned LoRA adapters
│       ├── adapter_config.json
│       ├── adapter_model.safetensors
│       └── checkpoint-XXX/  # Training checkpoints
├── outputs/
│   ├── logs/                # TensorBoard logs
│   └── evaluation_results.json
```

## Câu hỏi Thường gặp

**Q: Tôi có thể dừng training và tiếp tục sau không?**
A: Có, sử dụng `--resume_from_checkpoint` với checkpoint gần nhất.

**Q: Model có hoạt động offline không?**
A: Sau khi tải xong, model có thể chạy hoàn toàn offline.

**Q: Tôi có thể fine-tune thêm trên model đã train không?**
A: Có, chỉ cần tiếp tục training với data mới.

**Q: Làm sao để deploy model?**
A: Có thể tích hợp vào FastAPI hoặc sử dụng trực tiếp qua inference script.
