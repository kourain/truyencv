"""
Script đánh giá chất lượng model
"""

import json
import torch
import yaml
import argparse
from pathlib import Path
from tqdm import tqdm
from typing import List, Dict
from transformers import AutoModelForCausalLM, AutoTokenizer
from peft import PeftModel
import numpy as np


class ModelEvaluator:
    """Đánh giá chất lượng model"""

    def __init__(self, model_path: str, config_path: str):
        self.model_path = Path(model_path)

        with open(config_path, "r", encoding="utf-8") as f:
            self.config = yaml.safe_load(f)

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

        self.model = AutoModelForCausalLM.from_pretrained(
            base_model_name,
            device_map="auto",
            torch_dtype=torch.bfloat16,
            trust_remote_code=True,
        )

        # Load LoRA adapters
        self.model = PeftModel.from_pretrained(
            self.model,
            self.model_path,
        )

        self.model.eval()
        print("Model đã sẵn sàng!")

    def load_test_data(self, test_file: str) -> List[Dict]:
        """Load test dataset"""
        data = []
        with open(test_file, "r", encoding="utf-8") as f:
            for line in f:
                data.append(json.loads(line))
        return data

    def format_prompt(self, sample: Dict) -> str:
        """Format prompt"""
        instruction = sample.get("instruction", "")
        input_text = sample.get("input", "")
        system = sample.get("system", "")

        prompt = f"""<|im_start|>system
{system}<|im_end|>
<|im_start|>user
{instruction}

{input_text}<|im_end|>
<|im_start|>assistant
"""

        return prompt

    def generate(self, sample: Dict) -> str:
        """Generate prediction"""
        prompt = self.format_prompt(sample)

        inputs = self.tokenizer(
            prompt,
            return_tensors="pt",
            truncation=True,
            max_length=1024,
        ).to(self.device)

        with torch.no_grad():
            outputs = self.model.generate(
                **inputs,
                max_new_tokens=512,
                temperature=0.7,
                top_p=0.9,
                top_k=50,
                repetition_penalty=1.1,
                do_sample=True,
                pad_token_id=self.tokenizer.pad_token_id,
                eos_token_id=self.tokenizer.eos_token_id,
            )

        generated_text = self.tokenizer.decode(outputs[0], skip_special_tokens=True)

        # Extract response
        if "<|im_start|>assistant" in generated_text:
            response = generated_text.split("<|im_start|>assistant")[-1]
            if "<|im_end|>" in response:
                response = response.split("<|im_end|>")[0]
            return response.strip()

        return generated_text

    def calculate_bleu(self, reference: str, hypothesis: str) -> float:
        """Tính BLEU score đơn giản (unigram)"""
        ref_words = set(reference.split())
        hyp_words = set(hypothesis.split())

        if len(hyp_words) == 0:
            return 0.0

        matches = len(ref_words.intersection(hyp_words))
        return matches / len(hyp_words)

    def calculate_metrics(self, predictions: List[str], references: List[str]) -> Dict:
        """Tính các metrics"""
        bleu_scores = []

        for pred, ref in zip(predictions, references):
            bleu = self.calculate_bleu(ref, pred)
            bleu_scores.append(bleu)

        metrics = {
            "bleu_mean": np.mean(bleu_scores),
            "bleu_std": np.std(bleu_scores),
            "num_samples": len(predictions),
        }

        return metrics

    def evaluate(
        self, test_file: str, output_file: str = None, num_samples: int = None
    ):
        """Đánh giá model trên test set"""
        print(f"Đang load test data từ {test_file}...")
        test_data = self.load_test_data(test_file)

        if num_samples:
            test_data = test_data[:num_samples]

        print(f"Đánh giá trên {len(test_data)} samples...")

        predictions = []
        references = []
        results = []

        for sample in tqdm(test_data, desc="Evaluating"):
            # Generate prediction
            pred = self.generate(sample)
            ref = sample["output"]

            predictions.append(pred)
            references.append(ref)

            # Store result
            results.append(
                {
                    "input": sample["input"],
                    "reference": ref,
                    "prediction": pred,
                    "bleu": self.calculate_bleu(ref, pred),
                }
            )

        # Calculate overall metrics
        metrics = self.calculate_metrics(predictions, references)

        print("\n" + "=" * 50)
        print("KẾT QUẢ ĐÁNH GIÁ")
        print("=" * 50)
        print(f"Số samples: {metrics['num_samples']}")
        print(f"BLEU Score (mean): {metrics['bleu_mean']:.4f}")
        print(f"BLEU Score (std): {metrics['bleu_std']:.4f}")
        print("=" * 50)

        # Save results
        if output_file:
            output_path = Path(output_file)
            output_path.parent.mkdir(parents=True, exist_ok=True)

            with open(output_path, "w", encoding="utf-8") as f:
                json.dump(
                    {
                        "metrics": metrics,
                        "results": results,
                    },
                    f,
                    ensure_ascii=False,
                    indent=2,
                )

            print(f"\nĐã lưu kết quả vào: {output_path}")

        # Print some examples
        print("\n" + "=" * 50)
        print("MỘT SỐ VÍ DỤ")
        print("=" * 50)

        for i in range(min(3, len(results))):
            print(f"\nVí dụ {i + 1}:")
            print(f"Input: {results[i]['input'][:100]}...")
            print(f"Reference: {results[i]['reference'][:100]}...")
            print(f"Prediction: {results[i]['prediction'][:100]}...")
            print(f"BLEU: {results[i]['bleu']:.4f}")

        return metrics, results


def main():
    parser = argparse.ArgumentParser(description="Đánh giá model GPT-OSS đã fine-tune")
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
        "--test_file", type=str, default="data/test.jsonl", help="File test data"
    )
    parser.add_argument(
        "--output_file",
        type=str,
        default="outputs/evaluation_results.json",
        help="File lưu kết quả đánh giá",
    )
    parser.add_argument(
        "--num_samples",
        type=int,
        default=None,
        help="Số lượng samples để đánh giá (None = tất cả)",
    )

    args = parser.parse_args()

    # Create evaluator
    evaluator = ModelEvaluator(args.model_path, args.config)

    # Run evaluation
    evaluator.evaluate(
        test_file=args.test_file,
        output_file=args.output_file,
        num_samples=args.num_samples,
    )


if __name__ == "__main__":
    main()
