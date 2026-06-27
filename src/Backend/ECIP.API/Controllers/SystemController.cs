using Microsoft.AspNetCore.Mvc;
using ECIP.Shared.DTOs;

namespace ECIP.API.Controllers;

/// <summary>
/// System status endpoint controller.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SystemController : ControllerBase
{
    /// <summary>
    /// Gets the system services status.
    /// </summary>
    /// <returns>System status response with all service statuses.</returns>
    [HttpGet]
    public IActionResult GetStatus()
    {
        var response = new ApiResponse<SystemStatusResponse>(
            new SystemStatusResponse
            {
                WebApi = "Healthy",
                RepositoryService = "Offline",
                AIService = "Offline",
                PromptRegistry = "Offline",
                MCP = "Offline",
                Gateway = "Offline"
            }
        );

        return Ok(response);
    }
}
