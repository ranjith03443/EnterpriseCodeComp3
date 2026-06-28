from fastapi import APIRouter, HTTPException
from pydantic import BaseModel
from registry.prompt_registry import PromptRegistry
from dependencies import get_registry

router = APIRouter(prefix="/registry", tags=["Prompt Registry"])


class PromptSummary(BaseModel):
    name: str
    version: str
    description: str
    parameters: list[str]
    tags: list[str]


class PromptDetail(PromptSummary):
    template: str


@router.get("/prompts", response_model=list[PromptSummary])
async def list_prompts():
    registry = get_registry()
    return [
        PromptSummary(
            name=p.name,
            version=p.version,
            description=p.description,
            parameters=p.parameters,
            tags=p.tags,
        )
        for p in registry.list_all()
    ]


@router.get("/prompts/{name}", response_model=PromptDetail)
async def get_prompt(name: str):
    registry = get_registry()
    prompt = registry.get(name)
    if prompt is None:
        raise HTTPException(status_code=404, detail=f"Prompt '{name}' not found")
    return PromptDetail(
        name=prompt.name,
        version=prompt.version,
        description=prompt.description,
        parameters=prompt.parameters,
        tags=prompt.tags,
        template=prompt.template,
    )
