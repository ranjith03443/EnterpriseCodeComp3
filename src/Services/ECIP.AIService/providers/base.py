from abc import ABC, abstractmethod
from dataclasses import dataclass, field


@dataclass
class CompletionRequest:
    prompt: str
    system: str = ""
    max_tokens: int = 2000
    temperature: float = 0.7


@dataclass
class CompletionResponse:
    content: str
    provider: str
    model: str
    input_tokens: int
    output_tokens: int
    duration_ms: int


@dataclass
class EmbeddingRequest:
    text: str


@dataclass
class EmbeddingResponse:
    vector: list[float]
    provider: str
    model: str
    dimensions: int
    duration_ms: int


class ILlmProvider(ABC):
    @abstractmethod
    async def complete(self, request: CompletionRequest) -> CompletionResponse:
        pass

    @property
    @abstractmethod
    def provider_name(self) -> str:
        pass

    @property
    @abstractmethod
    def model_name(self) -> str:
        pass


class IEmbeddingProvider(ABC):
    @abstractmethod
    async def embed(self, request: EmbeddingRequest) -> EmbeddingResponse:
        pass

    @property
    @abstractmethod
    def provider_name(self) -> str:
        pass

    @property
    @abstractmethod
    def model_name(self) -> str:
        pass
