namespace ECIP.Web.ViewModels;

using ECIP.Core.Entities;
using ECIP.Shared.DTOs;

/// <summary>
/// View model for the repository details page with tabs.
/// </summary>
public class RepositoryDetailsViewModel
{
    public RepositoryEntity Repository { get; set; } = new();
    public RepositoryMetadataSummaryDto? Summary { get; set; }
    public RepositoryStatisticsDto? Statistics { get; set; }
    public List<RepositoryProjectDto> Projects { get; set; } = new();
    public List<LanguageSummaryDto> Languages { get; set; } = new();
    public List<FolderStatisticsDto> FolderStatistics { get; set; } = new();
    public string ActiveTab { get; set; } = "summary";
    public bool HasMetadata => Summary != null;
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
}
