using System.Text;
using System.Text.Json;
using ECIP.Core.Interfaces.AI;
using ECIP.Shared.DTOs.AI;

namespace ECIP.API.Services;

public class AiGatewayClient : IAiGatewayClient
{
    private readonly HttpClient _http;
    private readonly ILogger<AiGatewayClient> _logger;
    private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

    public AiGatewayClient(HttpClient http, ILogger<AiGatewayClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<bool> IsAvailableAsync()
    {
        try
        {
            var response = await _http.GetAsync("/health");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<AiCompletionResponse?> CompleteAsync(AiCompletionRequest request)
        => await PostAsync<AiCompletionRequest, AiCompletionResponse>("/gateway/complete", request);

    public async Task<AiCompletionResponse?> CompleteRawAsync(AiRawCompletionRequest request)
        => await PostAsync<AiRawCompletionRequest, AiCompletionResponse>("/gateway/complete/raw", request);

    public async Task<AiEmbeddingResponse?> EmbedAsync(AiEmbeddingRequest request)
        => await PostAsync<AiEmbeddingRequest, AiEmbeddingResponse>("/gateway/embed", request);

    public async Task<AiSettingsDto?> GetSettingsAsync()
        => await GetAsync<AiSettingsDto>("/settings");

    public async Task<AiSettingsDto?> SaveSettingsAsync(AiSettingsDto settings)
        => await PutAsync<AiSettingsDto, AiSettingsDto>("/settings", settings);

    public async Task<List<ConnectionTestResultDto>?> TestConnectionAsync()
        => await PostAsync<object, List<ConnectionTestResultDto>>("/settings/test", new { });

    public async Task<List<PromptDto>?> GetPromptsAsync()
        => await GetAsync<List<PromptDto>>("/registry/prompts");

    public async Task<PromptDetailDto?> GetPromptAsync(string name)
        => await GetAsync<PromptDetailDto>($"/registry/prompts/{name}");

    private async Task<TResponse?> GetAsync<TResponse>(string path)
    {
        try
        {
            var response = await _http.GetAsync(path);
            if (!response.IsSuccessStatusCode) return default;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(json, _json);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "AI service GET {Path} failed", path);
            return default;
        }
    }

    private async Task<TResponse?> PostAsync<TRequest, TResponse>(string path, TRequest body)
    {
        try
        {
            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var response = await _http.PostAsync(path, content);
            if (!response.IsSuccessStatusCode) return default;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(json, _json);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "AI service POST {Path} failed", path);
            return default;
        }
    }

    private async Task<TResponse?> PutAsync<TRequest, TResponse>(string path, TRequest body)
    {
        try
        {
            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var response = await _http.PutAsync(path, content);
            if (!response.IsSuccessStatusCode) return default;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(json, _json);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "AI service PUT {Path} failed", path);
            return default;
        }
    }
}
