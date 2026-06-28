from fastapi import APIRouter
from pydantic import BaseModel
import time

router = APIRouter(prefix="/health", tags=["Health"])


class HealthResponse(BaseModel):
    status: str
    service: str
    timestamp: str


@router.get("", response_model=HealthResponse)
async def health():
    return HealthResponse(
        status="healthy",
        service="ECIP AI Service",
        timestamp=time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
    )
