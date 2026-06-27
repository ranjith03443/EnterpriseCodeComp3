namespace ECIP.Shared.DTOs;

/// <summary>
/// System services status response DTO.
/// </summary>
public class SystemStatusResponse
{
    /// <summary>
    /// Gets or sets the status of the Web API service.
    /// </summary>
    public string WebApi { get; set; } = "Healthy";

    /// <summary>
    /// Gets or sets the status of the Repository Service.
    /// </summary>
    public string RepositoryService { get; set; } = "Offline";

    /// <summary>
    /// Gets or sets the status of the AI Service.
    /// </summary>
    public string AIService { get; set; } = "Offline";

    /// <summary>
    /// Gets or sets the status of the Prompt Registry.
    /// </summary>
    public string PromptRegistry { get; set; } = "Offline";

    /// <summary>
    /// Gets or sets the status of the MCP (Model Context Protocol).
    /// </summary>
    public string MCP { get; set; } = "Offline";

    /// <summary>
    /// Gets or sets the status of the Gateway.
    /// </summary>
    public string Gateway { get; set; } = "Offline";
}
