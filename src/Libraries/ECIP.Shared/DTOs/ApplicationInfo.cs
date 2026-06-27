namespace ECIP.Shared.DTOs;

/// <summary>
/// Application information DTO.
/// </summary>
public class ApplicationInfo
{
    /// <summary>
    /// Gets or sets the application name.
    /// </summary>
    public string Name { get; set; } = "Enterprise Code Intelligence Platform";

    /// <summary>
    /// Gets or sets the application version.
    /// </summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// Gets or sets the target framework.
    /// </summary>
    public string Framework { get; set; } = ".NET 8";

    /// <summary>
    /// Gets or sets the deployment environment.
    /// </summary>
    public string Environment { get; set; } = "Development";

    /// <summary>
    /// Gets or sets the application description.
    /// </summary>
    public string Description { get; set; } = "AI-powered code comprehension platform for enterprise repositories";
}
