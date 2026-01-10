# Linux bash
source .cuda_venv
CUDA_LAUNCH_BLOCKING=1 TORCH_USE_CUDA_DSA=1 uv run --no-sync app.py