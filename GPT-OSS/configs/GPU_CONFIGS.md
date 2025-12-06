# GPU-specific Training Configurations
# Chọn config phù hợp với GPU của bạn

## RTX 3050 Laptop (4GB VRAM) - Default
# Sử dụng: training_config.yaml

## Google Colab T4 (15GB VRAM)
# Sử dụng: training_config_t4.yaml
# - Tăng batch size lên 2
# - Giảm gradient accumulation xuống 8
# - Tăng max_seq_length lên 2048
# - Training nhanh hơn ~2x so với RTX 3050

## Google Colab V100 (16GB VRAM)
# Sử dụng: training_config_v100.yaml
# - Batch size: 4
# - Gradient accumulation: 4
# - Max_seq_length: 2048
# - Training nhanh hơn ~3x so với RTX 3050

## Google Colab A100 (40GB VRAM) - Colab Pro
# Sử dụng: training_config_a100.yaml
# - Batch size: 8
# - Gradient accumulation: 2
# - Max_seq_length: 4096
# - Có thể dùng LoRA rank cao hơn (32)
# - Training nhanh hơn ~5x so với RTX 3050

## Desktop RTX 4090 (24GB VRAM)
# Sử dụng: training_config_4090.yaml
# - Batch size: 4
# - Gradient accumulation: 4
# - Max_seq_length: 2048
# - Training nhanh hơn ~4x so với RTX 3050

---

## Cách chọn config

```bash
# RTX 3050 (default)
python scripts/train.py --config configs/training_config.yaml

# Google Colab T4
python scripts/train.py --config configs/training_config_t4.yaml

# Google Colab V100
python scripts/train.py --config configs/training_config_v100.yaml

# Google Colab A100
python scripts/train.py --config configs/training_config_a100.yaml

# RTX 4090
python scripts/train.py --config configs/training_config_4090.yaml
```

---

## Auto-detect GPU

Script sẽ tự động detect GPU và đề xuất config phù hợp:

```bash
python scripts/detect_gpu.py
```

---

## Ước tính thời gian training

| GPU | VRAM | Batch Size | Time/Epoch | Total (3 epochs) |
|-----|------|------------|------------|------------------|
| RTX 3050 | 4GB | 1 | ~24h | ~3 days |
| Colab T4 | 15GB | 2 | ~12h | ~1.5 days |
| Colab V100 | 16GB | 4 | ~8h | ~1 day |
| Colab A100 | 40GB | 8 | ~5h | ~15 hours |
| RTX 4090 | 24GB | 4 | ~6h | ~18 hours |

**Lưu ý**: Colab Free có giới hạn 12h/session, cần resume training nhiều lần.
