using ECIP.Shared.DTOs.AI;

namespace ECIP.Core.Interfaces.AI;

public interface IAiGatewayClient
{
    Task<AiCompletionResponse?> CompleteAsync(AiCompletionRequest request);
    Task<AiCompletionResponse?> CompleteRawAsync(AiRawCompletionRequest request);
    Task<AiEmbeddingResponse?> EmbedAsync(AiEmbeddingRequest request);
    Task<AiSettingsDto?> GetSettingsAsync();
    Task<AiSettingsDto?> SaveSettingsAsync(AiSettingsDto settings);
    Task<List<ConnectionTestResultDto>?> TestConnectionAsync();
    Task<List<PromptDto>?> GetPromptsAsync();
    Task<PromptDetailDto?> GetPromptAsync(string name);
    Task<bool> IsAvailableAsync();
}
