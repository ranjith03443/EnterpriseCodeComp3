using ECIP.Core.Interfaces;
using ECIP.Web.Services;
using ECIP.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ECIP.Web.Controllers;

/// <summary>
/// Repository Intelligence Dashboard controller.
/// Displays repository metadata, statistics, languages, projects, and folder exploration.
/// </summary>
public class RepositoryIntelligenceController : Controller
{
    private readonly IApiService _apiService;
    private readonly IRepositoryService _repositoryService;
    private readonly ILogger<RepositoryIntelligenceController> _logger;

    public RepositoryIntelligenceController(
        IApiService apiService,
        IRepositoryService repositoryService,
        ILogger<RepositoryIntelligenceController> logger)
    {
        _apiService = apiService;
        _repositoryService = repositoryService;
        _logger = logger;
    }

    /// <summary>
    /// Displays the Repository Intelligence Dashboard for the active repository.
    /// </summary>
    public async Task<IActionResult> Index(Guid? id, string tab = "overview", string? search = null, string? sortBy = null, string? filterLanguage = null, string? filterProjectType = null)
    {
        var viewModel = new RepositoryIntelligenceViewModel { ActiveTab = tab };

        try
        {
            // Resolve repository ID
            Guid repositoryId;
            if (id.HasValue)
            {
                repositoryId = id.Value;
            }
            else
            {
                var repositories = await _repositoryService.GetRepositoriesAsync();
                var active = repositories.FirstOrDefault(r => r.IsActive);
                if (active is null)
                {
                    viewModel.ErrorMessage = "No active repository. Please register and activate a repository first.";
                    return View(viewModel);
                }
                repositoryId = active.Id;
            }

            viewModel.RepositoryId = repositoryId;
            viewModel.SearchQuery = search;
            viewModel.SortBy = sortBy;
            viewModel.FilterLanguage = filterLanguage;
            viewModel.FilterProjectType = filterProjectType;
            viewModel.ExportJsonUrl = _apiService.GetExportJsonUrl(repositoryId);
            viewModel.ExportCsvUrl = _apiService.GetExportCsvUrl(repositoryId);

            // Load dashboard data from API
            var dashboard = await _apiService.GetRepositoryDashboardAsync(repositoryId);
            if (dashboard != null)
            {
                // Apply search filter to projects
                if (!string.IsNullOrWhiteSpace(search))
                {
                    dashboard.Projects = dashboard.Projects
                        .Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                    p.ProjectType.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                    p.RelativePath.Contains(search, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                // Apply language filter
                if (!string.IsNullOrWhiteSpace(filterLanguage))
                {
                    dashboard.Languages = dashboard.Languages
                        .Where(l => l.Language.Contains(filterLanguage, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                // Apply project type filter
                if (!string.IsNullOrWhiteSpace(filterProjectType))
                {
                    dashboard.Projects = dashboard.Projects
                        .Where(p => p.ProjectType.Contains(filterProjectType, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                // Apply sorting to projects
                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    dashboard.Projects = sortBy.ToLower() switch
                    {
                        "name" => dashboard.Projects.OrderBy(p => p.Name).ToList(),
                        "type" => dashboard.Projects.OrderBy(p => p.ProjectType).ToList(),
                        "path" => dashboard.Projects.OrderBy(p => p.RelativePath).ToList(),
                        _ => dashboard.Projects
                    };
                }

                viewModel.Dashboard = dashboard;
            }

            _logger.LogInformation("Dashboard loaded for repository {RepositoryId}", repositoryId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading repository intelligence dashboard");
            viewModel.ErrorMessage = "Error loading dashboard data. Ensure the API is running.";
        }

        return View(viewModel);
    }
}
