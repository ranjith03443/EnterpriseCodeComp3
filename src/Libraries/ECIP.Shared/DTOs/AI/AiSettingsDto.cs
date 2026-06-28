namespace ECIP.Shared.DTOs.AI;

public class ProviderSettingsDto
{
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
}

public class AiSettingsDto
{
    public string ActiveProvider { get; set; } = "mock";
    public string ActiveEmbeddingProvider { get; set; } = "mock";
    public int MockDelayMs { get; set; } = 500;
    public Dictionary<string, ProviderSettingsDto> Providers { get; set; } = new();
    public Dictionary<string, ProviderSettingsDto> Embeddings { get; set; } = new();
}

public class ConnectionTestResultDto
{
    public string Component { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int DurationMs { get; set; }
}
