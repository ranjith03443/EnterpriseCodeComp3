using ECIP.Shared.DTOs.AI;

namespace ECIP.Web.ViewModels;

public class AiSettingsViewModel
{
    public string ActiveProvider { get; set; } = "mock";
    public string ActiveEmbeddingProvider { get; set; } = "mock";
    public int MockDelayMs { get; set; } = 500;

    public string OpenAiApiKey { get; set; } = string.Empty;
    public string OpenAiModel { get; set; } = "gpt-4o";
    public string OpenAiEndpoint { get; set; } = string.Empty;

    public string GeminiApiKey { get; set; } = string.Empty;
    public string GeminiModel { get; set; } = "gemini-1.5-pro";
    public string GeminiEndpoint { get; set; } = string.Empty;

    public string OllamaModel { get; set; } = "llama3.2";
    public string OllamaEndpoint { get; set; } = "http://localhost:11434";

    public string OpenAiEmbeddingApiKey { get; set; } = string.Empty;
    public string OpenAiEmbeddingModel { get; set; } = "text-embedding-ada-002";
    public string OllamaEmbeddingModel { get; set; } = "nomic-embed-text";
    public string OllamaEmbeddingEndpoint { get; set; } = "http://localhost:11434";

    public bool AiServiceAvailable { get; set; }
    public string SaveMessage { get; set; } = string.Empty;
    public bool SaveSuccess { get; set; }

    public static AiSettingsViewModel FromDto(AiSettingsDto dto)
    {
        var vm = new AiSettingsViewModel
        {
            ActiveProvider = dto.ActiveProvider,
            ActiveEmbeddingProvider = dto.ActiveEmbeddingProvider,
            MockDelayMs = dto.MockDelayMs,
            AiServiceAvailable = true,
        };

        if (dto.Providers.TryGetValue("openai", out var openai))
        {
            vm.OpenAiApiKey = openai.ApiKey;
            vm.OpenAiModel = openai.Model;
            vm.OpenAiEndpoint = openai.Endpoint;
        }
        if (dto.Providers.TryGetValue("gemini", out var gemini))
        {
            vm.GeminiApiKey = gemini.ApiKey;
            vm.GeminiModel = gemini.Model;
            vm.GeminiEndpoint = gemini.Endpoint;
        }
        if (dto.Providers.TryGetValue("ollama", out var ollama))
        {
            vm.OllamaModel = ollama.Model;
            vm.OllamaEndpoint = ollama.Endpoint;
        }
        if (dto.Embeddings.TryGetValue("openai", out var oembed))
        {
            vm.OpenAiEmbeddingApiKey = oembed.ApiKey;
            vm.OpenAiEmbeddingModel = oembed.Model;
        }
        if (dto.Embeddings.TryGetValue("ollama", out var olembed))
        {
            vm.OllamaEmbeddingModel = olembed.Model;
            vm.OllamaEmbeddingEndpoint = olembed.Endpoint;
        }
        return vm;
    }

    public AiSettingsDto ToDto() => new()
    {
        ActiveProvider = ActiveProvider,
        ActiveEmbeddingProvider = ActiveEmbeddingProvider,
        MockDelayMs = MockDelayMs,
        Providers = new()
        {
            ["openai"] = new ProviderSettingsDto { ApiKey = OpenAiApiKey, Model = OpenAiModel, Endpoint = OpenAiEndpoint },
            ["gemini"] = new ProviderSettingsDto { ApiKey = GeminiApiKey, Model = GeminiModel, Endpoint = GeminiEndpoint },
            ["ollama"] = new ProviderSettingsDto { Model = OllamaModel, Endpoint = OllamaEndpoint },
            ["mock"] = new ProviderSettingsDto(),
        },
        Embeddings = new()
        {
            ["openai"] = new ProviderSettingsDto { ApiKey = OpenAiEmbeddingApiKey, Model = OpenAiEmbeddingModel },
            ["ollama"] = new ProviderSettingsDto { Model = OllamaEmbeddingModel, Endpoint = OllamaEmbeddingEndpoint },
            ["mock"] = new ProviderSettingsDto(),
        },
    };
}
