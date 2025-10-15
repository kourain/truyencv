from typing import Dict, Any
from pydantic import BaseModel
from datetime import datetime

class UploadResponse(BaseModel):
    success: bool
    message: str
    data: Dict[str, Any] | None = None
    url: str | None = None
    created_at: datetime = datetime.utcnow()

class DeleteResponse(BaseModel):
    success: bool
    message: str
    
class ErrorResponse(BaseModel):
    success: bool = False
    message: str
    error: str | None = None