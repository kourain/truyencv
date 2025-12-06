# Quick Start Script - GPT-OSS Fine-tuning
# Chạy script này để bắt đầu nhanh

Write-Host "=" * 60 -ForegroundColor Cyan
Write-Host "GPT-OSS FINE-TUNING - QUICK START" -ForegroundColor Cyan
Write-Host "=" * 60 -ForegroundColor Cyan
Write-Host ""

# Check Python version
Write-Host "[1/5] Kiểm tra Python version..." -ForegroundColor Yellow
$pythonVersion = python --version 2>&1
Write-Host "Python version: $pythonVersion" -ForegroundColor Green

if ($pythonVersion -notmatch "3.11") {
    Write-Host "WARNING: Yêu cầu Python 3.11.x" -ForegroundColor Red
    Write-Host "Bạn có muốn tiếp tục? (y/n)" -ForegroundColor Yellow
    $continue = Read-Host
    if ($continue -ne "y") {
        exit
    }
}

# Check CUDA
Write-Host ""
Write-Host "[2/5] Kiểm tra CUDA..." -ForegroundColor Yellow
try {
    $cudaVersion = nvidia-smi --query-gpu=driver_version --format=csv,noheader 2>&1
    Write-Host "CUDA Driver: $cudaVersion" -ForegroundColor Green
} catch {
    Write-Host "WARNING: Không tìm thấy CUDA. Training sẽ rất chậm trên CPU!" -ForegroundColor Red
}

# Install dependencies
Write-Host ""
Write-Host "[3/5] Cài đặt dependencies..." -ForegroundColor Yellow
Write-Host "Quá trình này có thể mất 10-20 phút..." -ForegroundColor Gray
Write-Host ""

$installChoice = Read-Host "Bạn có muốn cài đặt dependencies ngay bây giờ? (y/n)"
if ($installChoice -eq "y") {
    uv pip install -e .
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Cài đặt thành công!" -ForegroundColor Green
    } else {
        Write-Host "✗ Cài đặt thất bại. Vui lòng kiểm tra lỗi ở trên." -ForegroundColor Red
        exit
    }
} else {
    Write-Host "Bỏ qua cài đặt. Nhớ chạy 'uv pip install -e .' trước khi training!" -ForegroundColor Yellow
}

# Check data
Write-Host ""
Write-Host "[4/5] Kiểm tra dữ liệu..." -ForegroundColor Yellow

$truyenCount = (Get-ChildItem -Path "truyen" -Directory -ErrorAction SilentlyContinue).Count
if ($truyenCount -gt 0) {
    Write-Host "✓ Tìm thấy $truyenCount truyện trong thư mục truyen/" -ForegroundColor Green
} else {
    Write-Host "✗ Không tìm thấy dữ liệu trong thư mục truyen/" -ForegroundColor Red
    Write-Host "Vui lòng đảm bảo các truyện đã được đặt trong thư mục truyen/" -ForegroundColor Yellow
}

# Prepare data
Write-Host ""
Write-Host "[5/5] Chuẩn bị dữ liệu..." -ForegroundColor Yellow

$prepareChoice = Read-Host "Bạn có muốn chuẩn bị dữ liệu ngay bây giờ? (y/n)"
if ($prepareChoice -eq "y") {
    Write-Host "Đang xử lý $truyenCount truyện..." -ForegroundColor Gray
    Write-Host "Quá trình này có thể mất 30-60 phút..." -ForegroundColor Gray
    Write-Host ""
    
    python scripts/prepare_data.py --input_dir truyen/ --output_dir data/
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Chuẩn bị dữ liệu thành công!" -ForegroundColor Green
        
        # Show stats
        $trainSamples = (Get-Content "data/train.jsonl" | Measure-Object -Line).Lines
        $valSamples = (Get-Content "data/val.jsonl" | Measure-Object -Line).Lines
        $testSamples = (Get-Content "data/test.jsonl" | Measure-Object -Line).Lines
        
        Write-Host ""
        Write-Host "Dataset Statistics:" -ForegroundColor Cyan
        Write-Host "  Train: $trainSamples samples" -ForegroundColor White
        Write-Host "  Val:   $valSamples samples" -ForegroundColor White
        Write-Host "  Test:  $testSamples samples" -ForegroundColor White
    } else {
        Write-Host "✗ Chuẩn bị dữ liệu thất bại." -ForegroundColor Red
        exit
    }
} else {
    Write-Host "Bỏ qua chuẩn bị dữ liệu. Nhớ chạy 'python scripts/prepare_data.py' trước khi training!" -ForegroundColor Yellow
}

# Summary
Write-Host ""
Write-Host "=" * 60 -ForegroundColor Cyan
Write-Host "SETUP HOÀN TẤT!" -ForegroundColor Green
Write-Host "=" * 60 -ForegroundColor Cyan
Write-Host ""
Write-Host "Bước tiếp theo:" -ForegroundColor Yellow
Write-Host "  1. Review dữ liệu trong thư mục data/" -ForegroundColor White
Write-Host "  2. Chạy training: python scripts/train.py" -ForegroundColor White
Write-Host "  3. Monitor với TensorBoard: tensorboard --logdir outputs/logs" -ForegroundColor White
Write-Host ""
Write-Host "Xem thêm hướng dẫn chi tiết trong USAGE.md" -ForegroundColor Gray
Write-Host ""

# Ask if want to start training
$trainChoice = Read-Host "Bạn có muốn bắt đầu training ngay bây giờ? (y/n)"
if ($trainChoice -eq "y") {
    Write-Host ""
    Write-Host "Bắt đầu training..." -ForegroundColor Green
    Write-Host "Lưu ý: Quá trình này sẽ mất 2-3 ngày. Bạn có thể dừng bằng Ctrl+C và resume sau." -ForegroundColor Yellow
    Write-Host ""
    
    Start-Sleep -Seconds 3
    python scripts/train.py --config configs/training_config.yaml
} else {
    Write-Host "Chúc bạn thành công! 🚀" -ForegroundColor Cyan
}
