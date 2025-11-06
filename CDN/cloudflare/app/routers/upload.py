from fastapi import APIRouter, UploadFile, HTTPException, Depends, File
from app.schemas import UploadResponse, DeleteResponse, ErrorResponse
from app.r2 import r2_client
import os
from datetime import datetime
from typing import List
import mimetypes

router = APIRouter()

def validate_file_type(content_type: str) -> bool:
    """
    Validate if the file type is allowed
    """
    allowed_types = [
        'image/jpeg', 'image/png', 'image/gif', 'image/webp',
        'application/pdf', 'application/zip',
        'text/plain', 'application/json'
    ]
    return content_type in allowed_types

def get_file_extension(content_type: str) -> str:
    """
    Get file extension from content type
    """
    extension = mimetypes.guess_extension(content_type)
    return extension if extension else ''

@router.post("/upload", response_model=UploadResponse, tags=["files"])
async def upload_file(file: UploadFile = File(...)):
    """
    Upload a file to Cloudflare R2
    """
    try:
        # Validate file type
        if not validate_file_type(file.content_type):
            raise HTTPException(
                status_code=400,
                detail=f"File type {file.content_type} not allowed"
            )

        # Read file content
        file_content = await file.read()
        
        # Generate unique filename
        timestamp = datetime.utcnow().strftime('%Y%m%d_%H%M%S')
        extension = get_file_extension(file.content_type)
        filename = f"{timestamp}_{file.filename}"

        # Upload to R2
        url = await r2_client.upload_file(
            file_data=file_content,
            file_name=filename,
            content_type=file.content_type
        )

        return UploadResponse(
            success=True,
            message="File uploaded successfully",
            data={
                "filename": filename,
                "content_type": file.content_type,
                "size": len(file_content)
            },
            url=url
        )

    except Exception as e:
        raise HTTPException(
            status_code=500,
            detail=str(e)
        )

@router.delete("/{filename}", response_model=DeleteResponse, tags=["files"])
async def delete_file(filename: str):
    """
    Delete a file from Cloudflare R2
    """
    try:
        success = await r2_client.delete_file(filename)
        return DeleteResponse(
            success=success,
            message="File deleted successfully"
        )
    except Exception as e:
        raise HTTPException(
            status_code=500,
            detail=str(e)
        )