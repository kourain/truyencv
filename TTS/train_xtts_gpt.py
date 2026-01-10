#!/usr/bin/env python3
"""
Script để fine-tune XTTS model sử dụng GPTTrainer.
Dựa trên TTS/demos/xtts_ft_demo/utils/gpt_train.py
"""

import os
import sys
from pathlib import Path
from trainer import Trainer, TrainerArgs

from TTS.config.shared_configs import BaseDatasetConfig
from TTS.tts.datasets import load_tts_samples
from TTS.tts.layers.xtts.trainer.gpt_trainer import GPTArgs, GPTTrainer, GPTTrainerConfig, XttsAudioConfig
from TTS.utils.manage import ModelManager


def main():
    # Cấu hình đường dẫn
    PROJECT_ROOT = Path(__file__).parent
    DATASET_ROOT = PROJECT_ROOT / "data" / "ngochuyen_voice"
    OUTPUT_PATH = PROJECT_ROOT / "outputs" / "xtts_ngochuyen_ft"
    
    # Training Parameters
    LANGUAGE = "vi"
    NUM_EPOCHS = 10
    BATCH_SIZE = 1
    GRAD_ACCUM_STEPS = 3
    MAX_AUDIO_LENGTH = 455995  # ~20 seconds
    MAX_TEXT_LENGTH = 450
    # Logging parameters
    RUN_NAME = "xtts_ngochuyen_finetune"
    PROJECT_NAME = "XTTS_ngochuyen_ft"
    DASHBOARD_LOGGER = "tensorboard"
    LOGGER_URI = None
    
    # Set training output path
    OUT_PATH = OUTPUT_PATH / "run" / "training"
    OUT_PATH.mkdir(parents=True, exist_ok=True)
    
    # Training Parameters
    OPTIMIZER_WD_ONLY_ON_WEIGHTS = True  # for multi-gpu training set to False
    START_WITH_EVAL = False
    
    # Dataset configuration
    config_dataset = BaseDatasetConfig(
        formatter="coqui",
        dataset_name="ngochuyen_voice",
        path=str(DATASET_ROOT),
        meta_file_train="metadata_train.csv",
        meta_file_val="metadata_val.csv",
        language=LANGUAGE,
    )
    
    DATASETS_CONFIG_LIST = [config_dataset]
    
    # Define checkpoint paths
    CHECKPOINTS_OUT_PATH = OUT_PATH / "XTTS_v2.0_original_model_files"
    CHECKPOINTS_OUT_PATH.mkdir(parents=True, exist_ok=True)
    
    # DVAE files
    DVAE_CHECKPOINT_LINK = "https://coqui.gateway.scarf.sh/hf-coqui/XTTS-v2/main/dvae.pth"
    MEL_NORM_LINK = "https://coqui.gateway.scarf.sh/hf-coqui/XTTS-v2/main/mel_stats.pth"
    
    DVAE_CHECKPOINT = CHECKPOINTS_OUT_PATH / "dvae.pth"
    MEL_NORM_FILE = CHECKPOINTS_OUT_PATH / "mel_stats.pth"
    
    # Download DVAE files if needed
    if not DVAE_CHECKPOINT.exists() or not MEL_NORM_FILE.exists():
        print(" > Downloading DVAE files...")
        ModelManager._download_model_files(
            [MEL_NORM_LINK, DVAE_CHECKPOINT_LINK], 
            str(CHECKPOINTS_OUT_PATH), 
            progress_bar=True
        )
    
    # XTTS v2.0 files
    TOKENIZER_FILE_LINK = "https://coqui.gateway.scarf.sh/hf-coqui/XTTS-v2/main/vocab.json"
    XTTS_CHECKPOINT_LINK = "https://coqui.gateway.scarf.sh/hf-coqui/XTTS-v2/main/model.pth"
    XTTS_CONFIG_LINK = "https://coqui.gateway.scarf.sh/hf-coqui/XTTS-v2/main/config.json"
    
    TOKENIZER_FILE = Path("models") / "vocab.json"
    XTTS_CHECKPOINT = Path("models") / "model.pth"
    XTTS_CONFIG_FILE = Path("models") / "config.json"
    
    # Download XTTS v2.0 files if needed
    if not TOKENIZER_FILE.exists() or not XTTS_CHECKPOINT.exists():
        print(" > Downloading XTTS v2.0 files...")
        ModelManager._download_model_files(
            [TOKENIZER_FILE_LINK, XTTS_CHECKPOINT_LINK, XTTS_CONFIG_LINK],
            str(CHECKPOINTS_OUT_PATH),
            progress_bar=True
        )
    
    print("\n" + "="*50)
    print("XTTS Fine-tuning Configuration")
    print("="*50)
    print(f"Dataset: {DATASET_ROOT}")
    print(f"Language: {LANGUAGE}")
    print(f"Epochs: {NUM_EPOCHS}")
    print(f"Batch size: {BATCH_SIZE}")
    print(f"Output path: {OUT_PATH}")
    print("="*50 + "\n")
    
    # Init GPT model arguments
    model_args = GPTArgs(
        max_conditioning_length=132300,  # 6 secs
        min_conditioning_length=66150,   # 3 secs
        debug_loading_failures=False,
        max_wav_length=MAX_AUDIO_LENGTH,
        max_text_length=MAX_TEXT_LENGTH,
        mel_norm_file=str(MEL_NORM_FILE),
        dvae_checkpoint=str(DVAE_CHECKPOINT),
        xtts_checkpoint=str(XTTS_CHECKPOINT),
        tokenizer_file=str(TOKENIZER_FILE),
        gpt_num_audio_tokens=1026,
        gpt_start_audio_token=1024,
        gpt_stop_audio_token=1025,
        gpt_use_masking_gt_prompt_approach=True,
        gpt_use_perceiver_resampler=True,
    )
    
    # Define audio config
    audio_config = XttsAudioConfig(
        sample_rate=22050,
        dvae_sample_rate=22050,
        output_sample_rate=24000
    )
    
    # Training config
    config = GPTTrainerConfig(
        epochs=NUM_EPOCHS,
        output_path=str(OUT_PATH),
        model_args=model_args,
        run_name=RUN_NAME,
        project_name=PROJECT_NAME,
        run_description="XTTS GPT fine-tuning with ngochuyen_voice dataset",
        dashboard_logger=DASHBOARD_LOGGER,
        logger_uri=LOGGER_URI,
        audio=audio_config,
        batch_size=BATCH_SIZE,
        batch_group_size=48,
        eval_batch_size=BATCH_SIZE,
        num_loader_workers=8,
        eval_split_max_size=256,
        print_step=50,
        plot_step=100,
        log_model_step=100,
        save_step=1000,
        save_n_checkpoints=1,
        save_checkpoints=True,
        print_eval=False,
        optimizer="AdamW",
        optimizer_wd_only_on_weights=OPTIMIZER_WD_ONLY_ON_WEIGHTS,
        optimizer_params={"betas": [0.9, 0.96], "eps": 1e-8, "weight_decay": 1e-2},
        lr=5e-06,
        lr_scheduler="MultiStepLR",
        lr_scheduler_params={"milestones": [50000 * 18, 150000 * 18, 300000 * 18], "gamma": 0.5, "last_epoch": -1},
        test_sentences=[],
    )
    
    # Init model from config
    print(" > Initializing GPTTrainer model...")
    model = GPTTrainer.init_from_config(config)
    
    # Load training samples
    print(" > Loading training samples...")
    train_samples, eval_samples = load_tts_samples(
        DATASETS_CONFIG_LIST,
        eval_split=True,
        eval_split_max_size=config.eval_split_max_size,
        eval_split_size=config.eval_split_size,
    )
    
    print(f" > Train samples: {len(train_samples)}")
    print(f" > Eval samples: {len(eval_samples)}")
    
    # Init trainer
    print("\n > Initializing Trainer...")
    trainer = Trainer(
        TrainerArgs(
            restore_path=None,
            skip_train_epoch=False,
            start_with_eval=START_WITH_EVAL,
            grad_accum_steps=GRAD_ACCUM_STEPS,
        ),
        config,
        output_path=str(OUT_PATH),
        model=model,
        train_samples=train_samples,
        eval_samples=eval_samples,
    )
    
    print("\n" + "="*50)
    print("Starting training...")
    print("="*50 + "\n")
    print(f"TensorBoard: tensorboard --logdir={OUT_PATH}")
    print("\n")
    
    # Start training
    trainer.fit()
    
    print("\n" + "="*50)
    print("Training completed!")
    print(f"Checkpoints saved to: {OUT_PATH}")
    print("="*50)


if __name__ == "__main__":
    main()
