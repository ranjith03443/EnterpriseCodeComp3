using Microsoft.AspNetCore.Mvc;
using ECIP.Shared.DTOs;

namespace ECIP.API.Controllers;

/// <summary>
/// Version information endpoint controller.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class VersionController : ControllerBase
{
    /// <summary>
    /// Gets the application version information.
    /// </summary>
    /// <returns>Version information response.</returns>
    [HttpGet]
    public IActionResult GetVersion()
    {
        var response = new ApiResponse<VersionResponse>(
            new VersionResponse
            {
                Application = "Enterprise Code Intelligence Platform",
                Version = "1.0.0",
                Framework = ".NET 8",
                Environment = "Development"
            }
        );

        return Ok(response);
    }
}
