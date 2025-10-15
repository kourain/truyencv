import os
import boto3
from botocore.config import Config
from dotenv import load_dotenv

load_dotenv()

class R2Client:
    def __init__(self):
        self.account_id = os.getenv("CLOUDFLARE_ACCOUNT_ID")
        self.access_key_id = os.getenv("CLOUDFLARE_ACCESS_KEY_ID")
        self.secret_access_key = os.getenv("CLOUDFLARE_SECRET_ACCESS_KEY")
        self.bucket_name = os.getenv("CLOUDFLARE_BUCKET_NAME")
        
        if not all([self.account_id, self.access_key_id, self.secret_access_key, self.bucket_name]):
            raise ValueError("Missing required Cloudflare R2 configuration")

        self.endpoint_url = f"https://{self.account_id}.r2.cloudflarestorage.com"
        
        self.s3_client = boto3.client(
            service_name='s3',
            endpoint_url=self.endpoint_url,
            aws_access_key_id=self.access_key_id,
            aws_secret_access_key=self.secret_access_key,
            config=Config(
                region_name='auto',
                signature_version='s3v4',
            )
        )

    async def upload_file(self, file_data: bytes, file_name: str, content_type: str) -> str:
        """
        Upload a file to Cloudflare R2
        """
        try:
            self.s3_client.put_object(
                Bucket=self.bucket_name,
                Key=file_name,
                Body=file_data,
                ContentType=content_type
            )
            
            url = f"https://{self.bucket_name}.r2.dev/{file_name}"
            return url
            
        except Exception as e:
            raise Exception(f"Error uploading file to R2: {str(e)}")

    async def delete_file(self, file_name: str) -> bool:
        """
        Delete a file from Cloudflare R2
        """
        try:
            self.s3_client.delete_object(
                Bucket=self.bucket_name,
                Key=file_name
            )
            return True
        except Exception as e:
            raise Exception(f"Error deleting file from R2: {str(e)}")

    async def get_file_url(self, file_name: str) -> str:
        """
        Get the public URL for a file
        """
        return f"https://{self.bucket_name}.r2.dev/{file_name}"

# Singleton instance
r2_client = R2Client()