from fastapi import APIRouter
from pydantic import BaseModel
from typing import Optional
import time
import dependencies
from registry.prompt_registry import PromptRegistry

router = APIRouter(prefix="/api", tags=["onboarding"])
_registry = PromptRegistry()


class SourceChunk(BaseModel):
    id: str
    source: str
    content: str
    type: str
    score: float = 1.0


class OnboardingRequest(BaseModel):
    repository_id: str
    query: str
    context: str = ""
    chunks: list[SourceChunk] = []


class OnboardingResponse(BaseModel):
    answer: str
    sources: list[SourceChunk]
    intent: str
    duration_ms: int
    created_at: str


@router.post("/onboarding/ask", response_model=OnboardingResponse)
async def onboarding_ask(request: OnboardingRequest):
    start = time.time()
    gateway = dependencies.get_gateway()

    chunks_summary = "\n".join(
        f"[{c.type}] {c.source}: {c.content[:200]}" for c in request.chunks
    ) if request.chunks else "No specific chunks retrieved."

    prompt = _registry.render("onboarding_answer",
                              query=request.query,
                              context=request.context or "No additional context provided.",
                              chunks_summary=chunks_summary)

    result = await gateway.complete(prompt)
    duration = int((time.time() - start) * 1000)

    return OnboardingResponse(
        answer=result,
        sources=request.chunks[:5],
        intent="onboarding",
        duration_ms=duration,
        created_at=__import__("datetime").datetime.utcnow().isoformat() + "Z",
    )
