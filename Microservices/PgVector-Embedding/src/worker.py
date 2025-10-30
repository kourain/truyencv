from workers import WorkerEntrypoint
from functools import lru_cache
from typing import List

import os
# os.system("pip install sentence-transformers")
from fastapi import FastAPI, Request, HTTPException
from pydantic import BaseModel, Field
from sentence_transformers import SentenceTransformer

# Run: uv run app.py
class Default(WorkerEntrypoint):
    async def fetch(self, request):
        import asgi

        return await asgi.fetch(app, request.js_object, self.env)
    
app = FastAPI(title="PgVector Embedding Service", version="1.0.0")


@lru_cache(maxsize=1)
def get_model() -> SentenceTransformer:
    return SentenceTransformer("keepitreal/vietnamese-sbert")


class EmbedRequest(BaseModel):
    texts: List[str] = Field(default_factory=list, description="Danh sách đoạn văn cần embedding")


class EmbedResponse(BaseModel):
    embedding: List[float]
    dimensions: int


@app.get("/health")
async def health_check() -> dict:
    return {"status": "ok"}


@app.post("/embed", response_model=EmbedResponse)
async def embed(request: EmbedRequest) -> EmbedResponse:
    segments = [segment.strip() for segment in request.texts if segment and segment.strip()]
    if not segments:
        raise HTTPException(status_code=400, detail="texts must contain at least one non-empty string")

    model = get_model()
    embedding = model.encode(segments, normalize_embeddings=True)

    if len(segments) == 1:
        vector = embedding[0].tolist()
    else:
        vector = embedding.mean(axis=0).tolist()

    return EmbedResponse(embedding=vector, dimensions=len(vector))


