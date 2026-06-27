namespace ECIP.Core.Entities;

/// <summary>
/// Represents overall repository statistics generated after a scan.
/// </summary>
public class RepositoryStatistics
{
    public Guid Id { get; set; }
    public Guid RepositoryId { get; set; }
    public string RepositoryName { get; set; } = string.Empty;
    public string RepositoryType { get; set; } = string.Empty;
    public string RepositoryRoot { get; set; } = string.Empty;
    public int TotalFiles { get; set; }
    public int TotalFolders { get; set; }
    public int SourceFiles { get; set; }
    public int ConfigurationFiles { get; set; }
    public int DocumentationFiles { get; set; }
    public int ProjectCount { get; set; }
    public int LanguageCount { get; set; }
    public long TotalSizeBytes { get; set; }
    public string LargestFileName { get; set; } = string.Empty;
    public long LargestFileSize { get; set; }
    public DateTime? LastScanDate { get; set; }
    public string ScanDuration { get; set; } = string.Empty;
    public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;
}
