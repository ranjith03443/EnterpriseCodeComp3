namespace ECIP.Core.Interfaces.RepositoryIntelligence;

using ECIP.Core.Entities;

/// <summary>
/// Contract for repository scanning operations.
/// </summary>
public interface IRepositoryScanner
{
    /// <summary>
    /// Scans a repository and stores metadata.
    /// </summary>
    Task<RepositoryScanResult> ScanRepositoryAsync(Guid repositoryId);
}

/// <summary>
/// Result of a repository scan operation.
/// </summary>
public class RepositoryScanResult
{
    public Guid RepositoryId { get; set; }
    public DateTime ScanStarted { get; set; }
    public DateTime ScanCompleted { get; set; }
    public int FilesDiscovered { get; set; }
    public int FoldersDiscovered { get; set; }
    public string Duration { get; set; } = string.Empty;
}
