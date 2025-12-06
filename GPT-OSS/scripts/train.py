"""
Script fine-tuning GPT-OSS 20B với QLoRA
Tối ưu hóa cho RTX 3050 Laptop (4GB VRAM)
"""

import torch
import yaml
import argparse
from pathlib import Path
from typing import Dict
from datasets import load_dataset
from transformers import (
    AutoModelForCausalLM,
    AutoTokenizer,
    BitsAndBytesConfig,
    TrainingArguments,
)
from peft import LoraConfig, get_peft_model, prepare_model_for_kbit_training
from trl import SFTTrainer


class GPTOSSFineTuner:
    """Fine-tuner cho GPT-OSS 20B"""

    def __init__(self, config_path: str):
        with open(config_path, "r", encoding="utf-8") as f:
            self.config = yaml.safe_load(f)

        self.model_name = self.config["model"]["name"]
        self.output_dir = Path(self.config["training"]["output_dir"])
        self.output_dir.mkdir(parents=True, exist_ok=True)

        # Setup device
        self.device = "cuda" if torch.cuda.is_available() else "cpu"
        print(f"Sử dụng device: {self.device}")

        if self.device == "cuda":
            print(f"GPU: {torch.cuda.get_device_name(0)}")
            print(
                f"VRAM: {torch.cuda.get_device_properties(0).total_memory / 1024**3:.2f} GB"
            )

    def load_tokenizer(self):
        """Load tokenizer"""
        print(f"Đang load tokenizer từ {self.model_name}...")

        tokenizer = AutoTokenizer.from_pretrained(
            self.model_name,
            trust_remote_code=True,
            padding_side="right",
        )

        # Thêm pad token nếu chưa có
        if tokenizer.pad_token is None:
            tokenizer.pad_token = tokenizer.eos_token

        return tokenizer

    def load_model(self):
        """Load model với QLoRA configuration"""
        print(f"Đang load model {self.model_name} với 4-bit quantization...")

        # Quantization config
        bnb_config = BitsAndBytesConfig(
            load_in_4bit=self.config["quantization"]["load_in_4bit"],
            bnb_4bit_compute_dtype=getattr(
                torch, self.config["quantization"]["bnb_4bit_compute_dtype"]
            ),
            bnb_4bit_use_double_quant=self.config["quantization"][
                "bnb_4bit_use_double_quant"
            ],
            bnb_4bit_quant_type=self.config["quantization"]["bnb_4bit_quant_type"],
        )

        # Load model
        model = AutoModelForCausalLM.from_pretrained(
            self.model_name,
            quantization_config=bnb_config,
            device_map=self.config["model"]["device_map"],
            trust_remote_code=True,
            torch_dtype=getattr(torch, self.config["model"]["torch_dtype"]),
            cache_dir=self.config["model"].get("cache_dir"),
        )

        # Prepare for k-bit training
        model = prepare_model_for_kbit_training(model)

        return model

    def setup_lora(self, model):
        """Setup LoRA configuration"""
        print("Đang setup LoRA adapters...")

        lora_config = LoraConfig(
            r=self.config["lora"]["r"],
            lora_alpha=self.config["lora"]["lora_alpha"],
            lora_dropout=self.config["lora"]["lora_dropout"],
            target_modules=self.config["lora"]["target_modules"],
            bias=self.config["lora"]["bias"],
            task_type=self.config["lora"]["task_type"],
        )

        model = get_peft_model(model, lora_config)
        model.print_trainable_parameters()

        return model

    def load_datasets(self):
        """Load training datasets"""
        print("Đang load datasets...")

        data_files = {
            "train": self.config["data"]["train_file"],
            "validation": self.config["data"]["val_file"],
        }

        dataset = load_dataset("json", data_files=data_files)

        print(f"Train samples: {len(dataset['train'])}")
        print(f"Validation samples: {len(dataset['validation'])}")

        return dataset

    def format_instruction(self, sample: Dict) -> str:
        """Format sample theo template instruction"""
        instruction = sample.get("instruction", "")
        input_text = sample.get("input", "")
        output_text = sample.get("output", "")
        system = sample.get("system", "")

        # Format theo Harmony chat template của GPT-OSS
        prompt = f"""<|im_start|>system
{system}<|im_end|>
<|im_start|>user
{instruction}

{input_text}<|im_end|>
<|im_start|>assistant
{output_text}<|im_end|>"""

        return prompt

    def preprocess_function(self, examples, tokenizer):
        """Preprocess dataset"""
        texts = []
        for i in range(len(examples["instruction"])):
            sample = {
                "instruction": examples["instruction"][i],
                "input": examples["input"][i],
                "output": examples["output"][i],
                "system": examples["system"][i],
            }
            texts.append(self.format_instruction(sample))

        return tokenizer(
            texts,
            truncation=True,
            max_length=self.config["data"]["max_seq_length"],
            padding=False,
        )

    def get_training_arguments(self):
        """Get training arguments"""
        train_config = self.config["training"]

        return TrainingArguments(
            output_dir=train_config["output_dir"],
            num_train_epochs=train_config["num_train_epochs"],
            per_device_train_batch_size=train_config["per_device_train_batch_size"],
            per_device_eval_batch_size=train_config["per_device_eval_batch_size"],
            gradient_accumulation_steps=train_config["gradient_accumulation_steps"],
            gradient_checkpointing=train_config["gradient_checkpointing"],
            optim=train_config["optim"],
            learning_rate=train_config["learning_rate"],
            weight_decay=train_config["weight_decay"],
            warmup_ratio=train_config["warmup_ratio"],
            lr_scheduler_type=train_config["lr_scheduler_type"],
            max_grad_norm=train_config["max_grad_norm"],
            fp16=train_config["fp16"],
            bf16=train_config["bf16"],
            logging_steps=train_config["logging_steps"],
            logging_dir=train_config["logging_dir"],
            report_to=train_config["report_to"],
            save_strategy=train_config["save_strategy"],
            save_steps=train_config["save_steps"],
            save_total_limit=train_config["save_total_limit"],
            evaluation_strategy=train_config["evaluation_strategy"],
            eval_steps=train_config["eval_steps"],
            load_best_model_at_end=train_config["load_best_model_at_end"],
            metric_for_best_model=train_config["metric_for_best_model"],
            remove_unused_columns=train_config["remove_unused_columns"],
            ddp_find_unused_parameters=train_config["ddp_find_unused_parameters"],
            group_by_length=train_config["group_by_length"],
        )

    def train(self):
        """Main training loop"""
        print("=" * 50)
        print("BẮT ĐẦU FINE-TUNING GPT-OSS 20B")
        print("=" * 50)

        # Load tokenizer
        tokenizer = self.load_tokenizer()

        # Load model
        model = self.load_model()

        # Setup LoRA
        model = self.setup_lora(model)

        # Load datasets
        dataset = self.load_datasets()

        # Get training arguments
        training_args = self.get_training_arguments()

        # Create trainer
        print("Đang khởi tạo trainer...")
        trainer = SFTTrainer(
            model=model,
            args=training_args,
            train_dataset=dataset["train"],
            eval_dataset=dataset["validation"],
            tokenizer=tokenizer,
            formatting_func=lambda x: [self.format_instruction(sample) for sample in x],
            max_seq_length=self.config["data"]["max_seq_length"],
            packing=False,
        )

        # Start training
        print("\n" + "=" * 50)
        print("BẮT ĐẦU TRAINING...")
        print("=" * 50 + "\n")

        trainer.train()

        # Save final model
        print("\nĐang lưu model...")
        trainer.save_model()
        tokenizer.save_pretrained(self.output_dir)

        print("\n" + "=" * 50)
        print("HOÀN THÀNH FINE-TUNING!")
        print("=" * 50)
        print(f"Model đã được lưu tại: {self.output_dir}")


def main():
    parser = argparse.ArgumentParser(description="Fine-tune GPT-OSS 20B với QLoRA")
    parser.add_argument(
        "--config",
        type=str,
        default="configs/training_config.yaml",
        help="Đường dẫn đến file cấu hình",
    )
    parser.add_argument(
        "--resume_from_checkpoint",
        type=str,
        default=None,
        help="Resume training từ checkpoint",
    )

    args = parser.parse_args()

    # Create trainer
    trainer = GPTOSSFineTuner(args.config)

    # Start training
    trainer.train()


if __name__ == "__main__":
    main()
