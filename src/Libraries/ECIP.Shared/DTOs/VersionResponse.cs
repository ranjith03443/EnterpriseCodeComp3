namespace ECIP.Shared.DTOs;

/// <summary>
/// Application version information response DTO.
/// </summary>
public class VersionResponse
{
    /// <summary>
    /// Gets or sets the application name.
    /// </summary>
    public string Application { get; set; } = "Enterprise Code Intelligence Platform";

    /// <summary>
    /// Gets or sets the version number.
    /// </summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// Gets or sets the target framework.
    /// </summary>
    public string Framework { get; set; } = ".NET 8";

    /// <summary>
    /// Gets or sets the current environment.
    /// </summary>
    public string Environment { get; set; } = "Development";
}
