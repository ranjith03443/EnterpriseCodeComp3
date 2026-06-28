using ECIP.Core.Interfaces.AI;
using ECIP.Shared.DTOs;
using ECIP.Shared.DTOs.AI;
using Microsoft.AspNetCore.Mvc;

namespace ECIP.API.Controllers;

[ApiController]
[Route("api/ai")]
public class AiGatewayController : ControllerBase
{
    private readonly IAiGatewayClient _gateway;
    private readonly ILogger<AiGatewayController> _logger;

    public AiGatewayController(IAiGatewayClient gateway, ILogger<AiGatewayController> logger)
    {
        _gateway = gateway;
        _logger = logger;
    }

    [HttpGet("health")]
    public async Task<IActionResult> Health()
    {
        var available = await _gateway.IsAvailableAsync();
        var status = available ? "available" : "unavailable";
        return Ok(new ApiResponse<object>(new { status }, $"AI service is {status}."));
    }

    [HttpPost("complete")]
    public async Task<IActionResult> Complete([FromBody] AiCompletionRequest request)
    {
        var result = await _gateway.CompleteAsync(request);
        if (result is null)
            return StatusCode(503, new ApiResponse<string>("AI service unavailable.", false));
        return Ok(new ApiResponse<AiCompletionResponse>(result, "Completion successful."));
    }

    [HttpPost("complete/raw")]
    public async Task<IActionResult> CompleteRaw([FromBody] AiRawCompletionRequest request)
    {
        var result = await _gateway.CompleteRawAsync(request);
        if (result is null)
            return StatusCode(503, new ApiResponse<string>("AI service unavailable.", false));
        return Ok(new ApiResponse<AiCompletionResponse>(result, "Completion successful."));
    }

    [HttpPost("embed")]
    public async Task<IActionResult> Embed([FromBody] AiEmbeddingRequest request)
    {
        var result = await _gateway.EmbedAsync(request);
        if (result is null)
            return StatusCode(503, new ApiResponse<string>("AI service unavailable.", false));
        return Ok(new ApiResponse<AiEmbeddingResponse>(result, "Embedding successful."));
    }

    [HttpGet("settings")]
    public async Task<IActionResult> GetSettings()
    {
        var settings = await _gateway.GetSettingsAsync();
        if (settings is null)
            return StatusCode(503, new ApiResponse<string>("AI service unavailable.", false));
        return Ok(new ApiResponse<AiSettingsDto>(settings, "Settings retrieved."));
    }

    [HttpPut("settings")]
    public async Task<IActionResult> SaveSettings([FromBody] AiSettingsDto settings)
    {
        var result = await _gateway.SaveSettingsAsync(settings);
        if (result is null)
            return StatusCode(503, new ApiResponse<string>("AI service unavailable.", false));
        return Ok(new ApiResponse<AiSettingsDto>(result, "Settings saved."));
    }

    [HttpPost("settings/test")]
    public async Task<IActionResult> TestConnection()
    {
        var results = await _gateway.TestConnectionAsync();
        if (results is null)
            return StatusCode(503, new ApiResponse<string>("AI service unavailable.", false));
        return Ok(new ApiResponse<List<ConnectionTestResultDto>>(results, "Connection test complete."));
    }

    [HttpGet("prompts")]
    public async Task<IActionResult> GetPrompts()
    {
        var prompts = await _gateway.GetPromptsAsync();
        if (prompts is null)
            return StatusCode(503, new ApiResponse<string>("AI service unavailable.", false));
        return Ok(new ApiResponse<List<PromptDto>>(prompts, "Prompts retrieved."));
    }

    [HttpGet("prompts/{name}")]
    public async Task<IActionResult> GetPrompt(string name)
    {
        var prompt = await _gateway.GetPromptAsync(name);
        if (prompt is null)
            return NotFound(new ApiResponse<string>($"Prompt '{name}' not found.", false));
        return Ok(new ApiResponse<PromptDetailDto>(prompt, "Prompt retrieved."));
    }
}
