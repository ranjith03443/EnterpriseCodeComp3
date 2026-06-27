namespace ECIP.Web.ViewModels;

using ECIP.Shared.DTOs;

/// <summary>
/// View model for the Repository Intelligence Dashboard.
/// </summary>
public class RepositoryIntelligenceViewModel
{
    public Guid RepositoryId { get; set; }
    public RepositoryDashboardDto? Dashboard { get; set; }
    public string ActiveTab { get; set; } = "overview";
    public string? SearchQuery { get; set; }
    public string? SortBy { get; set; }
    public string? FilterLanguage { get; set; }
    public string? FilterProjectType { get; set; }
    public string ExportJsonUrl { get; set; } = string.Empty;
    public string ExportCsvUrl { get; set; } = string.Empty;
    public bool HasData => Dashboard != null && Dashboard.TotalFiles > 0;
    public string? ErrorMessage { get; set; }
}
