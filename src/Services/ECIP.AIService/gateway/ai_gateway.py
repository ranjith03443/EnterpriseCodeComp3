import asyncio
import logging
import time
from providers.base import ILlmProvider, IEmbeddingProvider, CompletionRequest, CompletionResponse, EmbeddingRequest, EmbeddingResponse
from registry.prompt_registry import PromptRegistry

logger = logging.getLogger("ecip.gateway")

_MAX_RETRIES = 3
_RETRY_DELAYS = [1.0, 2.0, 4.0]


class AiGateway:
    def __init__(
        self,
        llm_provider: ILlmProvider,
        embedding_provider: IEmbeddingProvider,
        prompt_registry: PromptRegistry,
    ):
        self._llm = llm_provider
        self._embeddings = embedding_provider
        self._registry = prompt_registry

    async def complete_with_prompt(
        self,
        prompt_name: str,
        parameters: dict,
        system: str = "",
        max_tokens: int = 2000,
        temperature: float = 0.7,
    ) -> CompletionResponse:
        rendered = self._registry.render(prompt_name, **parameters)
        return await self._complete(rendered, system, max_tokens, temperature)

    async def complete_raw(
        self,
        prompt: str,
        system: str = "",
        max_tokens: int = 2000,
        temperature: float = 0.7,
    ) -> CompletionResponse:
        return await self._complete(prompt, system, max_tokens, temperature)

    async def embed(self, text: str) -> EmbeddingResponse:
        start = time.monotonic()
        last_error: Exception | None = None
        for attempt, delay in enumerate((_RETRY_DELAYS[:_MAX_RETRIES])):
            try:
                request = EmbeddingRequest(text=text)
                result = await self._embeddings.embed(request)
                logger.info(
                    "Embed completed provider=%s model=%s dims=%d duration_ms=%d",
                    result.provider, result.model, result.dimensions, result.duration_ms,
                )
                return result
            except Exception as exc:
                last_error = exc
                logger.warning("Embed attempt %d failed: %s", attempt + 1, exc)
                if attempt < _MAX_RETRIES - 1:
                    await asyncio.sleep(delay)
        raise RuntimeError(f"Embedding failed after {_MAX_RETRIES} attempts: {last_error}") from last_error

    async def _complete(
        self, prompt: str, system: str, max_tokens: int, temperature: float
    ) -> CompletionResponse:
        last_error: Exception | None = None
        for attempt, delay in enumerate(_RETRY_DELAYS[:_MAX_RETRIES]):
            try:
                request = CompletionRequest(
                    prompt=prompt,
                    system=system,
                    max_tokens=max_tokens,
                    temperature=temperature,
                )
                result = await self._llm.complete(request)
                logger.info(
                    "Completion ok provider=%s model=%s in=%d out=%d duration_ms=%d",
                    result.provider, result.model,
                    result.input_tokens, result.output_tokens, result.duration_ms,
                )
                return result
            except Exception as exc:
                last_error = exc
                logger.warning("Completion attempt %d failed: %s", attempt + 1, exc)
                if attempt < _MAX_RETRIES - 1:
                    await asyncio.sleep(delay)
        raise RuntimeError(f"Completion failed after {_MAX_RETRIES} attempts: {last_error}") from last_error
