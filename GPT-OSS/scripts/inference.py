"""
Script inference - sử dụng model đã fine-tune để cải thiện văn bản
"""

import torch
import yaml
import argparse
from pathlib import Path
from transformers import AutoModelForCausalLM, AutoTokenizer
from peft import PeftModel


class GPTOSSInference:
    """Inference engine cho GPT-OSS đã fine-tune"""

    def __init__(self, model_path: str, config_path: str = None):
        self.model_path = Path(model_path)

        # Load config nếu có
        if config_path:
            with open(config_path, "r", encoding="utf-8") as f:
                self.config = yaml.safe_load(f)
        else:
            # Default config
            self.config = {
                "inference": {
                    "max_new_tokens": 512,
                    "temperature": 0.7,
                    "top_p": 0.9,
                    "top_k": 50,
                    "repetition_penalty": 1.1,
                    "do_sample": True,
                }
            }

        self.device = "cuda" if torch.cuda.is_available() else "cpu"
        print(f"Sử dụng device: {self.device}")

        self.load_model()

    def load_model(self):
        """Load model và tokenizer"""
        print(f"Đang load model từ {self.model_path}...")

        # Load tokenizer
        self.tokenizer = AutoTokenizer.from_pretrained(
            self.model_path,
            trust_remote_code=True,
        )

        if self.tokenizer.pad_token is None:
            self.tokenizer.pad_token = self.tokenizer.eos_token

        # Load base model
        base_model_name = "openai/gpt-oss-20b"

        print(f"Đang load base model {base_model_name}...")
        self.model = AutoModelForCausalLM.from_pretrained(
            base_model_name,
            device_map="auto",
            torch_dtype=torch.bfloat16,
            trust_remote_code=True,
        )

        # Load LoRA adapters
        print("Đang load LoRA adapters...")
        self.model = PeftModel.from_pretrained(
            self.model,
            self.model_path,
        )

        self.model.eval()
        print("Model đã sẵn sàng!")

    def format_prompt(self, text: str, instruction: str = None) -> str:
        """Format prompt cho inference"""
        if instruction is None:
            instruction = "Cải thiện chất lượng bản dịch sau đây, làm cho nó tự nhiên và văn học hơn:"

        system_prompt = "Bạn là một chuyên gia dịch thuật văn học, chuyên cải thiện chất lượng bản dịch truyện convert từ tiếng Trung sang tiếng Việt."

        prompt = f"""<|im_start|>system
{system_prompt}<|im_end|>
<|im_start|>user
{instruction}

{text}<|im_end|>
<|im_start|>assistant
"""

        return prompt

    def generate(self, text: str, instruction: str = None) -> str:
        """Generate improved text"""
        # Format prompt
        prompt = self.format_prompt(text, instruction)

        # Tokenize
        inputs = self.tokenizer(
            prompt,
            return_tensors="pt",
            truncation=True,
            max_length=1024,
        ).to(self.device)

        # Generate
        with torch.no_grad():
            outputs = self.model.generate(
                **inputs,
                max_new_tokens=self.config["inference"]["max_new_tokens"],
                temperature=self.config["inference"]["temperature"],
                top_p=self.config["inference"]["top_p"],
                top_k=self.config["inference"]["top_k"],
                repetition_penalty=self.config["inference"]["repetition_penalty"],
                do_sample=self.config["inference"]["do_sample"],
                pad_token_id=self.tokenizer.pad_token_id,
                eos_token_id=self.tokenizer.eos_token_id,
            )

        # Decode
        generated_text = self.tokenizer.decode(outputs[0], skip_special_tokens=True)

        # Extract only the assistant's response
        if "<|im_start|>assistant" in generated_text:
            response = generated_text.split("<|im_start|>assistant")[-1]
            if "<|im_end|>" in response:
                response = response.split("<|im_end|>")[0]
            return response.strip()

        return generated_text

    def improve_file(self, input_file: str, output_file: str):
        """Cải thiện toàn bộ file"""
        print(f"Đang xử lý file: {input_file}")

        with open(input_file, "r", encoding="utf-8") as f:
            content = f.read()

        # Split into paragraphs
        paragraphs = content.split("\n\n")
        improved_paragraphs = []

        for i, para in enumerate(paragraphs):
            if not para.strip():
                improved_paragraphs.append("")
                continue

            print(f"Đang xử lý đoạn {i + 1}/{len(paragraphs)}...")
            improved = self.generate(para.strip())
            improved_paragraphs.append(improved)

        # Save
        with open(output_file, "w", encoding="utf-8") as f:
            f.write("\n\n".join(improved_paragraphs))

        print(f"Đã lưu kết quả vào: {output_file}")


def main():
    parser = argparse.ArgumentParser(description="Inference với GPT-OSS đã fine-tune")
    parser.add_argument(
        "--model_path", type=str, required=True, help="Đường dẫn đến LoRA adapters"
    )
    parser.add_argument(
        "--config",
        type=str,
        default="configs/training_config.yaml",
        help="Đường dẫn đến file cấu hình",
    )
    parser.add_argument(
        "--input",
        type=str,
        default=None,
        help="Văn bản cần cải thiện (text hoặc file path)",
    )
    parser.add_argument("--input_file", type=str, default=None, help="File input")
    parser.add_argument("--output_file", type=str, default=None, help="File output")
    parser.add_argument("--interactive", action="store_true", help="Chế độ interactive")

    args = parser.parse_args()

    # Create inference engine
    engine = GPTOSSInference(args.model_path, args.config)

    # Interactive mode
    if args.interactive:
        print("\n" + "=" * 50)
        print("CHẾ ĐỘ INTERACTIVE")
        print("Nhập 'quit' hoặc 'exit' để thoát")
        print("=" * 50 + "\n")

        while True:
            text = input("\nNhập văn bản cần cải thiện:\n> ")

            if text.lower() in ["quit", "exit"]:
                break

            if not text.strip():
                continue

            print("\nĐang xử lý...")
            improved = engine.generate(text)

            print("\n" + "=" * 50)
            print("KẾT QUẢ:")
            print("=" * 50)
            print(improved)
            print("=" * 50)

    # File mode
    elif args.input_file and args.output_file:
        engine.improve_file(args.input_file, args.output_file)

    # Single text mode
    elif args.input:
        # Check if it's a file
        if Path(args.input).exists():
            with open(args.input, "r", encoding="utf-8") as f:
                text = f.read()
        else:
            text = args.input

        improved = engine.generate(text)
        print("\n" + "=" * 50)
        print("KẾT QUẢ:")
        print("=" * 50)
        print(improved)
        print("=" * 50)

    else:
        print("Vui lòng chỉ định --input, --input_file, hoặc --interactive")


if __name__ == "__main__":
    main()
