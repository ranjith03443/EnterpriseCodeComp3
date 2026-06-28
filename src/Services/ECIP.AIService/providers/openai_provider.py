import time
import httpx
from .base import ILlmProvider, CompletionRequest, CompletionResponse


class OpenAiProvider(ILlmProvider):
    def __init__(self, api_key: str, model: str = "gpt-4o", endpoint: str = ""):
        self._api_key = api_key
        self._model = model
        self._endpoint = endpoint or "https://api.openai.com/v1"

    @property
    def provider_name(self) -> str:
        return "openai"

    @property
    def model_name(self) -> str:
        return self._model

    async def complete(self, request: CompletionRequest) -> CompletionResponse:
        start = time.monotonic()
        messages = []
        if request.system:
            messages.append({"role": "system", "content": request.system})
        messages.append({"role": "user", "content": request.prompt})

        payload = {
            "model": self._model,
            "messages": messages,
            "max_tokens": request.max_tokens,
            "temperature": request.temperature,
        }

        async with httpx.AsyncClient(timeout=60) as client:
            response = await client.post(
                f"{self._endpoint}/chat/completions",
                headers={"Authorization": f"Bearer {self._api_key}", "Content-Type": "application/json"},
                json=payload,
            )
            response.raise_for_status()
            data = response.json()

        content = data["choices"][0]["message"]["content"]
        usage = data.get("usage", {})
        duration_ms = int((time.monotonic() - start) * 1000)

        return CompletionResponse(
            content=content,
            provider=self.provider_name,
            model=self._model,
            input_tokens=usage.get("prompt_tokens", 0),
            output_tokens=usage.get("completion_tokens", 0),
            duration_ms=duration_ms,
        )
