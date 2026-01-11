# Linux bash

source .venv/bin/activate

# Thiết đặt CUDA_HOME tự động từ nvcc (không hardcode)
if command -v nvcc >/dev/null 2>&1; then
  export CUDA_HOME="$(dirname "$(dirname "$(command -v nvcc)")")"
  export PATH="$CUDA_HOME/bin:$PATH"
  export LD_LIBRARY_PATH="$CUDA_HOME/lib64:${LD_LIBRARY_PATH:-}"
  export LD_LIBRARY_PATH="$VIRTUAL_ENV/lib/python3.11/site-packages/torch/lib:$VIRTUAL_ENV/lib/python3.11/site-packages/nvidia/cuda_runtime/lib:$VIRTUAL_ENV/lib/python3.11/site-packages/nvidia/cuda_cupti/lib:$VIRTUAL_ENV/lib/python3.11/site-packages/nvidia/cuda_nvrtc/lib:$VIRTUAL_ENV/lib/python3.11/site-packages/nvidia/cusparse/lib:$VIRTUAL_ENV/lib/python3.11/site-packages/nvidia/cudnn/lib:$VIRTUAL_ENV/lib/python3.11/site-packages/nvidia/cublas/lib:$VIRTUAL_ENV/lib/python3.11/site-packages/nvidia/curand/lib:$VIRTUAL_ENV/lib/python3.11/site-packages/nvidia/cusolver/lib:$VIRTUAL_ENV/lib/python3.11/site-packages/nvidia/cufft/lib:$VIRTUAL_ENV/lib/python3.11/site-packages/nvidia/cufile/lib:$VIRTUAL_ENV/lib/python3.11/site-packages/nvidia/nvjitlink/lib:$VIRTUAL_ENV/lib/python3.11/site-packages/nvidia/nvtx/lib:$VIRTUAL_ENV/lib/python3.11/site-packages/nvidia/npp/lib:$LD_LIBRARY_PATH"
else
  echo "Lỗi: không tìm thấy 'nvcc' (CUDA Toolkit) => CUDA_HOME không thể thiết lập đúng để build DeepSpeed op."
  echo "Bạn cần cài CUDA Toolkit (có nvcc)."
fi

# LD_LIBRARY_PATH bạn đang set để trỏ runtime CUDA trong venv (giữ nguyên)
source .venv/bin/activate && export LD_LIBRARY_PATH="$VIRTUAL_ENV/lib/python3.11/site-packages/torch/lib:$VIRTUAL_ENV/lib/python3.11/site-packages/nvidia/cuda_runtime/lib:$VIRTUAL_ENV/lib/python3.11/site-packages/nvidia/cuda_cupti/lib:$VIRTUAL_ENV/lib/python3.11/site-packages/nvidia/cuda_nvrtc/lib:$VIRTUAL_ENV/lib/python3.11/site-packages/nvidia/cusparse/lib:$VIRTUAL_ENV/lib/python3.11/site-packages/nvidia/cudnn/lib:$VIRTUAL_ENV/lib/python3.11/site-packages/nvidia/cublas/lib:$VIRTUAL_ENV/lib/python3.11/site-packages/nvidia/curand/lib:$VIRTUAL_ENV/lib/python3.11/site-packages/nvidia/cusolver/lib:$VIRTUAL_ENV/lib/python3.11/site-packages/nvidia/cufft/lib:$VIRTUAL_ENV/lib/python3.11/site-packages/nvidia/cufile/lib:$VIRTUAL_ENV/lib/python3.11/site-packages/nvidia/nvjitlink/lib:$VIRTUAL_ENV/lib/python3.11/site-packages/nvidia/nvtx/lib:$VIRTUAL_ENV/lib/python3.11/site-packages/nvidia/npp/lib:$LD_LIBRARY_PATH" && ldd .venv/lib/python3.11/site-packages/torchcodec/libtorchcodec_core6.so | grep 'not found'
# run
CUDA_LAUNCH_BLOCKING=1 TORCH_USE_CUDA_DSA=1 uv run --no-sync app.py