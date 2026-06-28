from fastapi import APIRouter
from pydantic import BaseModel
import time
import dependencies
from registry.prompt_registry import PromptRegistry

router = APIRouter(prefix="/api", tags=["codegen"])
_registry = PromptRegistry()


class CodeGenerationRequest(BaseModel):
    template: str
    language: str = "C#"
    name: str
    description: str = ""
    context: str = ""
    features: list[str] = []


class CodeGenerationResponse(BaseModel):
    code: str
    language: str
    template: str
    name: str
    files: list[str]
    explanation: str
    duration_ms: int
    created_at: str


@router.post("/codegen/generate", response_model=CodeGenerationResponse)
async def generate(request: CodeGenerationRequest):
    start = time.time()
    gateway = dependencies.get_gateway()

    prompt = _registry.render(
        "code_generation",
        template=request.template,
        language=request.language,
        name=request.name,
        description=request.description or f"A {request.template} named {request.name}",
        context=request.context or "No additional codebase context provided.",
        features=", ".join(request.features) if request.features else "Standard implementation",
    )

    code = await gateway.complete(prompt)
    duration = int((time.time() - start) * 1000)

    suggested_ext = {"C#": "cs", "Python": "py", "TypeScript": "ts",
                     "JavaScript": "js", "SQL": "sql"}.get(request.language, "txt")
    suggested_file = f"{request.name}.{suggested_ext}"

    explanation = (
        f"Generated a {request.template} named '{request.name}' in {request.language}. "
        f"The code follows clean architecture conventions with dependency injection and "
        f"appropriate error handling."
    )

    return CodeGenerationResponse(
        code=code,
        language=request.language,
        template=request.template,
        name=request.name,
        files=[suggested_file],
        explanation=explanation,
        duration_ms=duration,
        created_at=__import__("datetime").datetime.utcnow().isoformat() + "Z",
    )
