using Microsoft.AspNetCore.Mvc;
using ECIP.Shared.DTOs;

namespace ECIP.API.Controllers;

/// <summary>
/// Health check endpoint controller.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Gets the health status of the API.
    /// </summary>
    /// <returns>Health status response.</returns>
    [HttpGet]
    public IActionResult GetHealth()
    {
        var response = new ApiResponse<HealthResponse>(
            new HealthResponse
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow
            }
        );

        return Ok(response);
    }
}
