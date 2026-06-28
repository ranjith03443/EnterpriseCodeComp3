from fastapi import APIRouter, Depends, HTTPException
from pydantic import BaseModel
from gateway.ai_gateway import AiGateway
from dependencies import get_gateway

router = APIRouter(prefix="/gateway", tags=["Gateway"])


class PromptCompletionRequest(BaseModel):
    prompt_name: str
    parameters: dict = {}
    system: str = ""
    max_tokens: int = 2000
    temperature: float = 0.7


class RawCompletionRequest(BaseModel):
    prompt: str
    system: str = ""
    max_tokens: int = 2000
    temperature: float = 0.7


class CompletionResult(BaseModel):
    content: str
    provider: str
    model: str
    input_tokens: int
    output_tokens: int
    duration_ms: int


class EmbedRequest(BaseModel):
    text: str


class EmbedResult(BaseModel):
    vector: list[float]
    provider: str
    model: str
    dimensions: int
    duration_ms: int


@router.post("/complete", response_model=CompletionResult)
async def complete_with_prompt(request: PromptCompletionRequest, gateway: AiGateway = Depends(get_gateway)):
    try:
        result = await gateway.complete_with_prompt(
            prompt_name=request.prompt_name,
            parameters=request.parameters,
            system=request.system,
            max_tokens=request.max_tokens,
            temperature=request.temperature,
        )
        return CompletionResult(**result.__dict__)
    except KeyError as exc:
        raise HTTPException(status_code=404, detail=str(exc))
    except Exception as exc:
        raise HTTPException(status_code=500, detail=str(exc))


@router.post("/complete/raw", response_model=CompletionResult)
async def complete_raw(request: RawCompletionRequest, gateway: AiGateway = Depends(get_gateway)):
    try:
        result = await gateway.complete_raw(
            prompt=request.prompt,
            system=request.system,
            max_tokens=request.max_tokens,
            temperature=request.temperature,
        )
        return CompletionResult(**result.__dict__)
    except Exception as exc:
        raise HTTPException(status_code=500, detail=str(exc))


@router.post("/embed", response_model=EmbedResult)
async def embed(request: EmbedRequest, gateway: AiGateway = Depends(get_gateway)):
    try:
        result = await gateway.embed(request.text)
        return EmbedResult(**result.__dict__)
    except Exception as exc:
        raise HTTPException(status_code=500, detail=str(exc))
