namespace ECIP.Shared.DTOs;

/// <summary>
/// DTO for repository metadata summary response.
/// </summary>
public class RepositoryMetadataSummaryDto
{
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
    public string TotalSizeFormatted { get; set; } = string.Empty;
    public string LargestFileName { get; set; } = string.Empty;
    public long LargestFileSize { get; set; }
    public string LargestFileSizeFormatted { get; set; } = string.Empty;
    public DateTime? LastScanDate { get; set; }
    public string ScanDuration { get; set; } = string.Empty;
    public List<string> Languages { get; set; } = new();
}

/// <summary>
/// DTO for repository statistics response.
/// </summary>
public class RepositoryStatisticsDto
{
    public Guid RepositoryId { get; set; }
    public string RepositoryName { get; set; } = string.Empty;
    public int TotalFiles { get; set; }
    public int TotalFolders { get; set; }
    public int ProjectCount { get; set; }
    public int LanguageCount { get; set; }
    public long TotalSizeBytes { get; set; }
    public string TotalSizeFormatted { get; set; } = string.Empty;
    public List<LanguageSummaryDto> Languages { get; set; } = new();
    public List<RepositoryProjectDto> Projects { get; set; } = new();
    public List<FolderStatisticsDto> FolderStatistics { get; set; } = new();
}

/// <summary>
/// DTO for language summary.
/// </summary>
public class LanguageSummaryDto
{
    public string Language { get; set; } = string.Empty;
    public int FileCount { get; set; }
    public long TotalSize { get; set; }
    public string TotalSizeFormatted { get; set; } = string.Empty;
    public double Percentage { get; set; }
    public string Extension { get; set; } = string.Empty;
}

/// <summary>
/// DTO for discovered repository project.
/// </summary>
public class RepositoryProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ProjectType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string RelativePath { get; set; } = string.Empty;
    public string Directory { get; set; } = string.Empty;
}

/// <summary>
/// DTO for folder statistics.
/// </summary>
public class FolderStatisticsDto
{
    public string FolderName { get; set; } = string.Empty;
    public string ParentFolder { get; set; } = string.Empty;
    public string RelativePath { get; set; } = string.Empty;
    public int NumberOfFiles { get; set; }
    public long TotalSize { get; set; }
    public string TotalSizeFormatted { get; set; } = string.Empty;
    public int Depth { get; set; }
}
