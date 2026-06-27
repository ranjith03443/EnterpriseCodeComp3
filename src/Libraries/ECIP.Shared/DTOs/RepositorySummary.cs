namespace ECIP.Shared.DTOs;

/// <summary>
/// Repository summary information DTO.
/// </summary>
public class RepositorySummary
{
    /// <summary>
    /// Gets or sets the repository ID.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the repository name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the repository URL.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of files in the repository.
    /// </summary>
    public int FileCount { get; set; }

    /// <summary>
    /// Gets or sets the total lines of code.
    /// </summary>
    public int TotalLinesOfCode { get; set; }

    /// <summary>
    /// Gets or sets the programming languages used.
    /// </summary>
    public List<string> Languages { get; set; } = new();

    /// <summary>
    /// Gets or sets the last analysis timestamp.
    /// </summary>
    public DateTime? LastAnalyzed { get; set; }
}
