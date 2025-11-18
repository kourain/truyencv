"""FastAPI service exposing viXTTS inference based on the reference demo."""

import os
import string
from contextlib import asynccontextmanager
import datetime
from pathlib import Path

import torch
import torchaudio
from fastapi import FastAPI, Form, HTTPException
from fastapi.responses import FileResponse
from starlette.background import BackgroundTask
from starlette.concurrency import run_in_threadpool
from underthesea import sent_tokenize,text_normalize
from unidecode import unidecode
from vinorm import TTSnorm

from TTS.tts.configs.xtts_config import XttsConfig
from TTS.tts.models.xtts import Xtts


APP_TITLE = "viXTTS FastAPI"
SUMMARY = "REST API for Vietnamese XTTS inference (GPU-enabled)."

MODEL_DIR = "/home/kourain/truyencv/TTS/models"
VOICES_DIR = "/home/kourain/truyencv/TTS/voices"
OUTPUT_DIR = "/home/kourain/truyencv/TTS/outputs"
LANGUAGE = "vi"
REQUIRED_MODEL_FILES = {"model.pth", "config.json", "vocab.json"}
os.makedirs(OUTPUT_DIR, exist_ok=True)
# torch.cuda.amp.autocast(enabled=False)  # tự động tắt AMP để tránh lỗi OOM với một số GPU cũ eg: my 3050 :()

XTTS_MODEL: Xtts | None = None

def _clear_gpu_cache() -> None:
    if torch.cuda.is_available():
        torch.cuda.empty_cache()
def _load_model() -> Xtts:
    global XTTS_MODEL
    if XTTS_MODEL is not None:
        return XTTS_MODEL
        
    config = XttsConfig()
    config.load_json(f"{MODEL_DIR}/config.json")  # Cấu hình huấn luyện đã lưu của mô hình
    XTTS_MODEL = Xtts.init_from_config(config)
    XTTS_MODEL.load_checkpoint(
        config, checkpoint_dir=MODEL_DIR, use_deepspeed=False
    )
    if torch.cuda.is_available():
        XTTS_MODEL.cuda()

    XTTS_MODEL.eval()

    if torch.cuda.is_available():
        XTTS_MODEL.cuda()  # Chuyển mô hình sang GPU để suy luận
    else:
        raise RuntimeError("Không phát hiện được GPU. Hãy kiểm tra CUDA driver.")

    return XTTS_MODEL


def _normalize_text(text: str) -> str:
    text = text.replace("·","")
    open("temp.txt", "w", encoding="utf-8").write(text)
    cleaned = (
        text_normalize(text)
        .replace("..", ".")
        .replace("!.", "!")
        .replace("?.", "?")
        .replace(" .", ".")
        .replace(" ,", ",")
        .replace('"', "")
        .replace("'", "")
        .replace("AI", "Ây Ai")
        .replace("A.I", "Ây Ai")
    )
    return cleaned


def _calculate_keep_len(text: str, lang: str) -> int:
    if lang in {"ja", "zh-cn"}:
        return -1

    word_count = len(text.split())
    num_punct = sum(text.count(p) for p in [".", "!", "?", ","])

    if word_count < 5:
        return 15000 * word_count + 2000 * num_punct
    if word_count < 10:
        return 13000 * word_count + 2000 * num_punct
    return -1

def _filename_from_output_name(output_name: str,max_char: int = 50) -> str:
    snippet = output_name[:max_char].lower().replace(" ", "_")
    snippet = snippet.translate(str.maketrans("", "", string.punctuation.replace("_", "")))
    snippet = unidecode(snippet)
    return f"{snippet or 'tts'}"

def _filename_from_text(text: str, max_char: int = 50) -> str:
    snippet = text[:max_char].lower().replace(" ", "_")
    snippet = snippet.translate(str.maketrans("", "", string.punctuation.replace("_", "")))
    snippet = unidecode(snippet)
    timestamp = datetime.datetime.now(datetime.timezone.utc).strftime("%Y%m%d%H%M%S")
    return f"{timestamp}_{snippet or 'tts'}"


def _split_sentences(text: str) -> list[str]:
    return [s for s in sent_tokenize(text) if s.strip()]


def _infer(
    text: str,
    reference_path: Path,
    normalize_text: bool,
    output_name: str | None = None,
) -> str:
    model = XTTS_MODEL
    if model is None:
        raise RuntimeError("Mô hình chưa được khởi tạo")

    if normalize_text:
        text = _normalize_text(text)

    sentences = _split_sentences(text)
    if not sentences:
        raise ValueError("Nội dung văn bản trống sau khi xử lý.")

    gpt_cond_len = int(getattr(model.config, "gpt_cond_len", 0)) or 0
    max_ref_len = int(getattr(model.config, "max_ref_len", 0)) or 0
    sound_norm_refs = bool(getattr(model.config, "sound_norm_refs", True))

    conditioning = model.get_conditioning_latents(
        audio_path=str(reference_path),
        gpt_cond_len=gpt_cond_len,
        max_ref_length=max_ref_len,
        sound_norm_refs=sound_norm_refs,
    )

    gpt_cond_latent = conditioning[0]
    speaker_embedding = conditioning[1]

    wav_chunks = []
    for sentence in sentences:
        chunk = model.inference(  # type: ignore[call-arg]
            text=sentence,
            language=LANGUAGE,
            gpt_cond_latent=gpt_cond_latent,
            speaker_embedding=speaker_embedding,
            temperature=0.3,
            length_penalty=1.0,
            repetition_penalty=10.0,
            top_k=30,
            top_p=0.85,
            enable_text_splitting=True,
        )

        keep_len = _calculate_keep_len(sentence, LANGUAGE)
        audio_tensor = torch.from_numpy(chunk["wav"]).float()
        if keep_len > 0:
            audio_tensor = audio_tensor[:keep_len]
        wav_chunks.append(audio_tensor)

    audio = torch.cat(wav_chunks).unsqueeze(0)
    if output_name:
        output_name = f"{_filename_from_output_name(output_name)}.wav"
    else:
        output_name = f"{_filename_from_text(text)}.wav"
    output_path = f"{OUTPUT_DIR}/{output_name}"
    torchaudio.save(str(output_path), audio, 24000)
    return output_path


def _cleanup_file(path: str) -> None:
    try:
        os.unlink(path)
    except FileNotFoundError:
        pass


@asynccontextmanager
async def app_lifespan(_: FastAPI):
    _load_model()
    yield
    _clear_gpu_cache()

app = FastAPI(title=APP_TITLE, description=SUMMARY, lifespan=app_lifespan)

@app.get("/sounds")
async def list_sounds() -> list[str]:
    voices_path = Path(VOICES_DIR)
    if not voices_path.exists():
        raise HTTPException(status_code=500, detail="Thư mục voices không tồn tại")

    sound_files = [
        f.name for f in voices_path.iterdir() if f.is_file() and f.suffix.lower() == ".wav"
    ]
    return sound_files

@app.post("/tts")
async def synthesize(
    text: str = Form(..., description="Văn bản cần đọc"),
    normalize: bool = Form(True, description="Chuẩn hóa tiếng Việt trước khi đọc"),
    reference_audio: str = Form(..., description="Tên tệp WAV có sẵn trong thư mục voices"),
    output_name: str = Form(None, description="Tên tệp đầu ra (không bắt buộc)")
):
    if not reference_audio.strip():
        raise HTTPException(status_code=400, detail="Thiếu tên tệp mẫu giọng nói")

    reference_path = Path(VOICES_DIR) / reference_audio.strip()
    if not reference_path.suffix:
        reference_path = reference_path.with_suffix(".wav")
    if not reference_path.exists():
        raise HTTPException(status_code=400, detail="Không tìm thấy tệp mẫu trong thư mục voices")

    output_path = await run_in_threadpool(
        _infer,
        text,
        reference_path,
        normalize,
        output_name,
    )

    background_task = BackgroundTask(_cleanup_file, output_path)
    return FileResponse(
        path=output_path,
        filename=output_path,
        media_type="audio/wav",
        background=background_task,
    )


@app.get("/health")
async def health_check() -> dict[str, str]:
    device = "cuda" if torch.cuda.is_available() else "cpu"
    loaded = "yes" if XTTS_MODEL is not None else "no"
    return {"status": "ok", "device": device, "model_loaded": loaded}
if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="127.0.0.1", port=8000)