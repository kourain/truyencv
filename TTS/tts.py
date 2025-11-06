# Load model directly
from transformers import AutoModel
model = AutoModel.from_pretrained("capleaf/viXTTS", torch_dtype="auto")