namespace ECIP.Shared.DTOs;

/// <summary>
/// Health check response DTO.
/// </summary>
public class HealthResponse
{
    /// <summary>
    /// Gets or sets the health status.
    /// </summary>
    public string Status { get; set; } = "Healthy";

    /// <summary>
    /// Gets or sets the timestamp of the health check.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
