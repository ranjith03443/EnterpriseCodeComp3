import time
import httpx
from .base import ILlmProvider, CompletionRequest, CompletionResponse


class OllamaProvider(ILlmProvider):
    def __init__(self, model: str = "llama3.2", endpoint: str = "http://localhost:11434"):
        self._model = model
        self._endpoint = endpoint.rstrip("/")

    @property
    def provider_name(self) -> str:
        return "ollama"

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
            "stream": False,
            "options": {
                "num_predict": request.max_tokens,
                "temperature": request.temperature,
            },
        }

        async with httpx.AsyncClient(timeout=120) as client:
            response = await client.post(f"{self._endpoint}/api/chat", json=payload)
            response.raise_for_status()
            data = response.json()

        content = data["message"]["content"]
        duration_ms = int((time.monotonic() - start) * 1000)

        return CompletionResponse(
            content=content,
            provider=self.provider_name,
            model=self._model,
            input_tokens=data.get("prompt_eval_count", 0),
            output_tokens=data.get("eval_count", 0),
            duration_ms=duration_ms,
        )
