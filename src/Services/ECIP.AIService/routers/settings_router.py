import httpx
import asyncio
from fastapi import APIRouter, HTTPException
from pydantic import BaseModel
from config.settings import AppConfig
from dependencies import get_config, reload_gateway

router = APIRouter(prefix="/settings", tags=["Settings"])


class ProviderSettings(BaseModel):
    api_key: str = ""
    model: str = ""
    endpoint: str = ""


class AiSettings(BaseModel):
    active_provider: str
    active_embedding_provider: str
    mock_delay_ms: int
    providers: dict[str, ProviderSettings]
    embeddings: dict[str, ProviderSettings]


class ConnectionTestResult(BaseModel):
    component: str
    provider: str
    status: str
    message: str
    duration_ms: int


@router.get("", response_model=AiSettings)
async def get_settings():
    config = get_config()
    raw = config.to_dict()
    providers = {}
    for name, cfg in raw.get("providers", {}).items():
        api_key = cfg.get("api_key", "")
        masked = ("*" * (len(api_key) - 4) + api_key[-4:]) if len(api_key) > 4 else ("*" * len(api_key))
        providers[name] = ProviderSettings(
            api_key=masked if api_key else "",
            model=cfg.get("model", ""),
            endpoint=cfg.get("endpoint", ""),
        )
    embeddings = {}
    for name, cfg in raw.get("embeddings", {}).items():
        api_key = cfg.get("api_key", "")
        masked = ("*" * (len(api_key) - 4) + api_key[-4:]) if len(api_key) > 4 else ("*" * len(api_key))
        embeddings[name] = ProviderSettings(
            api_key=masked if api_key else "",
            model=cfg.get("model", ""),
            endpoint=cfg.get("endpoint", ""),
        )
    return AiSettings(
        active_provider=config.active_provider,
        active_embedding_provider=config.active_embedding_provider,
        mock_delay_ms=config.mock_delay_ms,
        providers=providers,
        embeddings=embeddings,
    )


@router.put("", response_model=AiSettings)
async def save_settings(settings: AiSettings):
    config = get_config()
    raw = config.to_dict()

    raw.setdefault("ai", {})["active_provider"] = settings.active_provider
    raw["ai"]["active_embedding_provider"] = settings.active_embedding_provider
    raw["ai"]["mock_delay_ms"] = settings.mock_delay_ms

    existing_providers = raw.get("providers", {})
    for name, ps in settings.providers.items():
        existing = existing_providers.get(name, {})
        api_key = ps.api_key if ps.api_key and "*" not in ps.api_key else existing.get("api_key", "")
        existing_providers[name] = {
            "api_key": api_key,
            "model": ps.model or existing.get("model", ""),
            "endpoint": ps.endpoint,
        }
    raw["providers"] = existing_providers

    existing_embed = raw.get("embeddings", {})
    for name, ps in settings.embeddings.items():
        existing = existing_embed.get(name, {})
        api_key = ps.api_key if ps.api_key and "*" not in ps.api_key else existing.get("api_key", "")
        existing_embed[name] = {
            "api_key": api_key,
            "model": ps.model or existing.get("model", ""),
            "endpoint": ps.endpoint,
        }
    raw["embeddings"] = existing_embed

    AppConfig.save_override(raw)
    reload_gateway()
    return await get_settings()


@router.post("/test", response_model=list[ConnectionTestResult])
async def test_connection():
    results: list[ConnectionTestResult] = []
    config = get_config()

    import time
    start = time.monotonic()
    results.append(ConnectionTestResult(
        component="AI Service",
        provider="FastAPI",
        status="connected",
        message="AI Service is running",
        duration_ms=int((time.monotonic() - start) * 1000),
    ))

    active = config.active_provider
    if active == "mock":
        results.append(ConnectionTestResult(
            component="LLM Provider",
            provider="Mock LLM",
            status="connected",
            message="Mock provider requires no connection",
            duration_ms=0,
        ))
    elif active == "ollama":
        cfg = config.provider_config("ollama")
        endpoint = cfg.get("endpoint", "http://localhost:11434")
        start = time.monotonic()
        try:
            async with httpx.AsyncClient(timeout=5) as client:
                r = await client.get(f"{endpoint}/api/tags")
            duration_ms = int((time.monotonic() - start) * 1000)
            if r.status_code == 200:
                results.append(ConnectionTestResult(component="LLM Provider", provider="Ollama", status="connected", message="Connected", duration_ms=duration_ms))
            else:
                results.append(ConnectionTestResult(component="LLM Provider", provider="Ollama", status="error", message=f"HTTP {r.status_code}", duration_ms=duration_ms))
        except Exception as exc:
            results.append(ConnectionTestResult(component="LLM Provider", provider="Ollama", status="error", message=str(exc), duration_ms=int((time.monotonic() - start) * 1000)))
    elif active in ("openai", "gemini"):
        results.append(ConnectionTestResult(component="LLM Provider", provider=active.title(), status="configured", message="API key configured. Live test not performed.", duration_ms=0))

    embed_active = config.active_embedding_provider
    if embed_active == "mock":
        results.append(ConnectionTestResult(component="Embedding Provider", provider="Mock", status="connected", message="Mock embeddings require no connection", duration_ms=0))
    elif embed_active == "ollama":
        cfg = config.embedding_config("ollama")
        endpoint = cfg.get("endpoint", "http://localhost:11434")
        start = time.monotonic()
        try:
            async with httpx.AsyncClient(timeout=5) as client:
                r = await client.get(f"{endpoint}/api/tags")
            duration_ms = int((time.monotonic() - start) * 1000)
            status = "connected" if r.status_code == 200 else "error"
            results.append(ConnectionTestResult(component="Embedding Provider", provider="Ollama", status=status, message="Connected" if status == "connected" else f"HTTP {r.status_code}", duration_ms=duration_ms))
        except Exception as exc:
            results.append(ConnectionTestResult(component="Embedding Provider", provider="Ollama", status="error", message=str(exc), duration_ms=0))
    elif embed_active == "openai":
        results.append(ConnectionTestResult(component="Embedding Provider", provider="OpenAI", status="configured", message="API key configured. Live test not performed.", duration_ms=0))

    return results
