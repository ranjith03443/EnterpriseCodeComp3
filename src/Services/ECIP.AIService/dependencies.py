from config.settings import AppConfig
from providers.base import ILlmProvider, IEmbeddingProvider
from providers.mock_provider import MockLlmProvider
from providers.openai_provider import OpenAiProvider
from providers.gemini_provider import GeminiProvider
from providers.ollama_provider import OllamaProvider
from embeddings.mock_embedding import MockEmbeddingProvider
from embeddings.openai_embedding import OpenAiEmbeddingProvider
from embeddings.ollama_embedding import OllamaEmbeddingProvider
from registry.prompt_registry import PromptRegistry
from gateway.ai_gateway import AiGateway

_config: AppConfig = AppConfig()
_registry: PromptRegistry = PromptRegistry()
_gateway: AiGateway | None = None


def _build_llm_provider(config: AppConfig) -> ILlmProvider:
    name = config.active_provider
    if name == "openai":
        cfg = config.provider_config("openai")
        return OpenAiProvider(api_key=cfg.get("api_key", ""), model=cfg.get("model", "gpt-4o"), endpoint=cfg.get("endpoint", ""))
    if name == "gemini":
        cfg = config.provider_config("gemini")
        return GeminiProvider(api_key=cfg.get("api_key", ""), model=cfg.get("model", "gemini-1.5-pro"), endpoint=cfg.get("endpoint", ""))
    if name == "ollama":
        cfg = config.provider_config("ollama")
        return OllamaProvider(model=cfg.get("model", "llama3.2"), endpoint=cfg.get("endpoint", "http://localhost:11434"))
    return MockLlmProvider(delay_ms=config.mock_delay_ms)


def _build_embedding_provider(config: AppConfig) -> IEmbeddingProvider:
    name = config.active_embedding_provider
    if name == "openai":
        cfg = config.embedding_config("openai")
        return OpenAiEmbeddingProvider(api_key=cfg.get("api_key", ""), model=cfg.get("model", "text-embedding-ada-002"), endpoint=cfg.get("endpoint", ""))
    if name == "ollama":
        cfg = config.embedding_config("ollama")
        return OllamaEmbeddingProvider(model=cfg.get("model", "nomic-embed-text"), endpoint=cfg.get("endpoint", "http://localhost:11434"))
    return MockEmbeddingProvider(delay_ms=100)


def _build_gateway(config: AppConfig) -> AiGateway:
    return AiGateway(
        llm_provider=_build_llm_provider(config),
        embedding_provider=_build_embedding_provider(config),
        prompt_registry=_registry,
    )


def initialise() -> None:
    global _config, _gateway
    _config = AppConfig()
    _gateway = _build_gateway(_config)


def reload_gateway() -> None:
    global _config, _gateway
    _config = AppConfig.reload()
    _gateway = _build_gateway(_config)


def get_config() -> AppConfig:
    return _config


def get_registry() -> PromptRegistry:
    return _registry


def get_gateway() -> AiGateway:
    if _gateway is None:
        raise RuntimeError("Gateway not initialised. Call initialise() first.")
    return _gateway
