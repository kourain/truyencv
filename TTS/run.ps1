# Windows PowerShell script to run TTS app with CUDA

$env:CUDA_LAUNCH_BLOCKING = "1"
$env:TORCH_USE_CUDA_DSA = "1"
$env:TORCHAUDIO_USE_BACKEND_DISPATCHER = "0"

# Set CUDA library paths for torch and nvidia packages
$pythonSitePackages = ".venv\Lib\site-packages"

$cudaPaths = @(
    "$pythonSitePackages\torch\lib",
    "$pythonSitePackages\nvidia\cuda_runtime\bin",
    "$pythonSitePackages\nvidia\cuda_cupti\bin",
    "$pythonSitePackages\nvidia\cuda_nvrtc\bin",
    "$pythonSitePackages\nvidia\cusparse\bin",
    "$pythonSitePackages\nvidia\cudnn\bin",
    "$pythonSitePackages\nvidia\cublas\bin",
    "$pythonSitePackages\nvidia\curand\bin",
    "$pythonSitePackages\nvidia\cusolver\bin",
    "$pythonSitePackages\nvidia\cufft\bin",
    "$pythonSitePackages\nvidia\cufile\bin",
    "$pythonSitePackages\nvidia\nvjitlink\bin",
    "$pythonSitePackages\nvidia\nvtx\bin",
    "$pythonSitePackages\nvidia\npp\bin"
)

# Check for FFmpeg installation
$ffmpegPath = (Get-Command ffmpeg -ErrorAction SilentlyContinue)
if ($ffmpegPath) {
    $ffmpegDir = Split-Path $ffmpegPath.Source
    Write-Host "Found FFmpeg at: $ffmpegDir" -ForegroundColor Cyan
} else {
    Write-Host "WARNING: FFmpeg not found in PATH. Installing via winget..." -ForegroundColor Yellow
    try {
        winget install --id=Gyan.FFmpeg -e --silent
        Write-Host "FFmpeg installed. Please restart your terminal or add FFmpeg to PATH manually." -ForegroundColor Green
        Write-Host "Typical location: C:\Program Files\FFmpeg\bin" -ForegroundColor Cyan
        exit 1
    } catch {
        Write-Host "ERROR: Could not install FFmpeg automatically." -ForegroundColor Red
        Write-Host "Please install manually from: https://www.gyan.dev/ffmpeg/builds/" -ForegroundColor Yellow
        Write-Host "Or via Chocolatey: choco install ffmpeg" -ForegroundColor Yellow
        exit 1
    }
}

$env:PATH = ($cudaPaths -join ";") + ";" + $env:PATH

Write-Host "Starting TTS FastAPI server with CUDA support..." -ForegroundColor Green
uv run --no-sync app.py
