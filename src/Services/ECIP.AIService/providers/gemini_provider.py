import time
import httpx
from .base import ILlmProvider, CompletionRequest, CompletionResponse


class GeminiProvider(ILlmProvider):
    def __init__(self, api_key: str, model: str = "gemini-1.5-pro", endpoint: str = ""):
        self._api_key = api_key
        self._model = model
        self._endpoint = endpoint or "https://generativelanguage.googleapis.com/v1beta"

    @property
    def provider_name(self) -> str:
        return "gemini"

    @property
    def model_name(self) -> str:
        return self._model

    async def complete(self, request: CompletionRequest) -> CompletionResponse:
        start = time.monotonic()
        parts = []
        if request.system:
            parts.append({"text": f"[System]: {request.system}\n\n[User]: {request.prompt}"})
        else:
            parts.append({"text": request.prompt})

        payload = {
            "contents": [{"parts": parts}],
            "generationConfig": {
                "maxOutputTokens": request.max_tokens,
                "temperature": request.temperature,
            },
        }

        url = f"{self._endpoint}/models/{self._model}:generateContent?key={self._api_key}"
        async with httpx.AsyncClient(timeout=60) as client:
            response = await client.post(url, json=payload)
            response.raise_for_status()
            data = response.json()

        content = data["candidates"][0]["content"]["parts"][0]["text"]
        usage = data.get("usageMetadata", {})
        duration_ms = int((time.monotonic() - start) * 1000)

        return CompletionResponse(
            content=content,
            provider=self.provider_name,
            model=self._model,
            input_tokens=usage.get("promptTokenCount", 0),
            output_tokens=usage.get("candidatesTokenCount", 0),
            duration_ms=duration_ms,
        )
