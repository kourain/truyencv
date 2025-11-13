
import os
from huggingface_hub import hf_hub_download, snapshot_download


os.system("python -m unidic download")
print(" > Tải mô hình...")
snapshot_download(repo_id="thinhlpg/viXTTS",
                  repo_type="model",
                  local_dir="model")
hf_hub_download(
    repo_id="coqui/XTTS-v2",
    filename="speakers_xtts.pth",
    local_dir="model",
)