from fastapi import FastAPI, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from app.routers import upload
import os
from dotenv import load_dotenv
import uvicorn

load_dotenv()

app = FastAPI(
    title="Cloudflare R2 Upload API",
    description="API for uploading files to Cloudflare R2 storage",
    version="1.0.0"
)

# CORS configuration
allowed_origins = os.getenv("ALLOWED_ORIGINS", "").split(",")
app.add_middleware(
    CORSMiddleware,
    allow_origins=allowed_origins,
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Include routers
app.include_router(upload.router, prefix="/api/files", tags=["files"])

@app.get("/health")
async def health_check():
    """
    Health check endpoint
    """
    return {"status": "healthy", "message": "Service is running"}
uvicorn_host = os.getenv("UVICORN_HOST","127.0.0.1")
uvicorn_port = os.getenv("UVICORN_PORT", "8000")
if __name__ == "__main__":
    uvicorn.run(app, host=uvicorn_host, port=int(uvicorn_port))