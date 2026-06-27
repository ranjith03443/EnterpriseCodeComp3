namespace ECIP.Core.Interfaces.RepositoryIntelligence;

using ECIP.Core.Entities;

/// <summary>
/// Contract for extracting repository metadata including summary, projects, and statistics.
/// </summary>
public interface IMetadataExtractionService
{
    /// <summary>
    /// Generates complete metadata for a repository after scanning.
    /// </summary>
    Task<RepositoryStatistics> GenerateMetadataAsync(Guid repositoryId);

    /// <summary>
    /// Gets the repository summary/statistics.
    /// </summary>
    Task<RepositoryStatistics?> GetRepositoryStatisticsAsync(Guid repositoryId);

    /// <summary>
    /// Gets discovered projects within a repository.
    /// </summary>
    Task<IReadOnlyList<RepositoryProject>> GetProjectsAsync(Guid repositoryId);

    /// <summary>
    /// Gets language summary with file counts, sizes, and percentages.
    /// </summary>
    Task<IReadOnlyList<LanguageSummary>> GetLanguageSummaryAsync(Guid repositoryId);

    /// <summary>
    /// Gets folder statistics for a repository.
    /// </summary>
    Task<IReadOnlyList<RepositoryFolderStatistics>> GetFolderStatisticsAsync(Guid repositoryId);
}

/// <summary>
/// Language summary with file count, size, and percentage.
/// </summary>
public class LanguageSummary
{
    public string Language { get; set; } = string.Empty;
    public int FileCount { get; set; }
    public long TotalSize { get; set; }
    public double Percentage { get; set; }
    public string Extension { get; set; } = string.Empty;
}
