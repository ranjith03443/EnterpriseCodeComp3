namespace ECIP.API.Configuration;

/// <summary>
/// API settings configuration.
/// </summary>
public class ApiSettings
{
    public const string SectionName = "Api";

    /// <summary>
    /// Gets or sets the API base URL.
    /// </summary>
    public string BaseUrl { get; set; } = "https://localhost:7001";

    /// <summary>
    /// Gets or sets the API version.
    /// </summary>
    public string Version { get; set; } = "1.0";

    /// <summary>
    /// Gets or sets the default page size for pagination.
    /// </summary>
    public int DefaultPageSize { get; set; } = 20;

    /// <summary>
    /// Gets or sets the maximum page size for pagination.
    /// </summary>
    public int MaxPageSize { get; set; } = 100;
}
