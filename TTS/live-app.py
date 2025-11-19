import logging
import os

import numpy as np
import torch
from TTS.app import _clear_gpu_cache
from TTS.tts.configs.xtts_config import XttsConfig
from TTS.tts.models.xtts import Xtts
from TTS.utils.generic_utils import get_user_data_dir
from TTS.utils.manage import ModelManager

# This is one of the speaker voices that comes with xtts
SPEAKER_NAME = "Claribel Dervla"
MODEL_DIR = "/home/kourain/truyencv/TTS/models"

class Model:
    def __init__(self, **kwargs):
        self.model:Xtts
        self.speaker: dict
        config = XttsConfig()
        config.load_json(f"{MODEL_DIR}/config.json")  # Cấu hình huấn luyện đã lưu của mô hình
        XTTS_MODEL = Xtts.init_from_config(config)
        XTTS_MODEL.load_checkpoint(
            config, checkpoint_dir=MODEL_DIR, use_deepspeed=False
        )
        if torch.cuda.is_available():
            _clear_gpu_cache()
            XTTS_MODEL.cuda()
            torch.backends.cuda.matmul.allow_tf32 = True  # Allow TF32 on Ampere
            torch.backends.cudnn.allow_tf32 = True
            # torch.backends.cudnn.benchmark = True # Enable cudnn autotuner
            try:
                torch.set_float32_matmul_precision("high")
            except AttributeError:
                pass # Old pytorch versions
        else:
            raise RuntimeError("Không phát hiện được GPU. Hãy kiểm tra CUDA driver.")

        XTTS_MODEL.eval()

        self.model = XTTS_MODEL
        self.speaker = {
            "speaker_embedding": self.model.speaker_manager.speakers[SPEAKER_NAME][
                "speaker_embedding"
            ]
            .cpu()
            .squeeze()
            .half()
            .tolist(),
            "gpt_cond_latent": self.model.speaker_manager.speakers[SPEAKER_NAME][
                "gpt_cond_latent"
            ]
            .cpu()
            .squeeze()
            .half()
            .tolist(),
        }

        self.speaker_embedding = (
            torch.tensor(self.speaker.get("speaker_embedding"))
            .unsqueeze(0)
            .unsqueeze(-1)
        )
        self.gpt_cond_latent = (
            torch.tensor(self.speaker.get("gpt_cond_latent"))
            .reshape((-1, 1024))
            .unsqueeze(0)
        )
        logging.info("🔥Model Loaded")

    def wav_postprocess(self, wav):
        """Post process the output waveform"""
        if isinstance(wav, list):
            wav = torch.cat(wav, dim=0)
        wav = wav.clone().detach().cpu().numpy()
        wav = np.clip(wav, -1, 1)
        wav = (wav * 32767).astype(np.int16)
        return wav

    def predict(self, model_input):
        text = model_input.get("text")
        language = model_input.get("language", "en")
        chunk_size = int(
            model_input.get("chunk_size", 20)
        )  # Ensure chunk_size is an integer
        add_wav_header = False

        streamer = self.model.inference_stream(
            text,
            language,
            self.gpt_cond_latent,
            self.speaker_embedding,
            stream_chunk_size=chunk_size,
            enable_text_splitting=True,
            temperature=0.2,
        )

        for chunk in streamer:
            processed_chunk = self.wav_postprocess(chunk)
            processed_bytes = processed_chunk.tobytes()
            yield processed_bytes
