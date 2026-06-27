namespace ECIP.API.Configuration;

/// <summary>
/// Logging settings configuration.
/// </summary>
public class LoggingSettings
{
    public const string SectionName = "Logging";

    /// <summary>
    /// Gets or sets the minimum log level.
    /// </summary>
    public string MinimumLevel { get; set; } = "Information";

    /// <summary>
    /// Gets or sets a value indicating whether to enable console logging.
    /// </summary>
    public bool EnableConsoleLogging { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to enable request/response logging.
    /// </summary>
    public bool EnableRequestLogging { get; set; } = true;

    /// <summary>
    /// Gets or sets the log output template.
    /// </summary>
    public string OutputTemplate { get; set; } = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}";
}
