namespace ECIP.Shared.DTOs;

/// <summary>
/// Aggregated dashboard data for repository intelligence view.
/// </summary>
public class RepositoryDashboardDto
{
    public Guid RepositoryId { get; set; }
    public string RepositoryName { get; set; } = string.Empty;
    public string RepositoryType { get; set; } = string.Empty;
    public string RepositoryRoot { get; set; } = string.Empty;
    public string RepositorySize { get; set; } = "0 B";
    public long RepositorySizeBytes { get; set; }
    public int TotalFiles { get; set; }
    public int TotalFolders { get; set; }
    public int SourceFiles { get; set; }
    public int ConfigurationFiles { get; set; }
    public int DocumentationFiles { get; set; }
    public int UnknownFiles { get; set; }
    public int ProjectCount { get; set; }
    public int LanguageCount { get; set; }
    public string LargestFileName { get; set; } = string.Empty;
    public string LargestFileSize { get; set; } = "0 B";
    public string AverageFileSize { get; set; } = "0 B";
    public DateTime? LastScanDate { get; set; }
    public string ScanDuration { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<LanguageSummaryDto> Languages { get; set; } = new();
    public List<RepositoryProjectDto> Projects { get; set; } = new();
    public List<FolderStatisticsDto> Folders { get; set; } = new();
    public List<RepositoryTimelineDto> Timeline { get; set; } = new();
}

/// <summary>
/// Repository scan timeline entry.
/// </summary>
public class RepositoryTimelineDto
{
    public DateTime ScanDate { get; set; }
    public string Duration { get; set; } = string.Empty;
    public int FilesProcessed { get; set; }
    public int FoldersProcessed { get; set; }
    public int ProjectsFound { get; set; }
    public int LanguagesDetected { get; set; }
    public string Status { get; set; } = "Completed";
}
