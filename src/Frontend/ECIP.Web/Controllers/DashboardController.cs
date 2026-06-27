using Microsoft.AspNetCore.Mvc;
using ECIP.Core.Interfaces;
using ECIP.Web.Services;
using ECIP.Web.ViewModels;

namespace ECIP.Web.Controllers;

/// <summary>
/// Dashboard controller for the main application page.
/// </summary>
public class DashboardController : Controller
{
    private readonly IApiService _apiService;
    private readonly IRepositoryService _repositoryService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IApiService apiService, IRepositoryService repositoryService, ILogger<DashboardController> logger)
    {
        _apiService = apiService;
        _repositoryService = repositoryService;
        _logger = logger;
    }

    /// <summary>
    /// Gets the dashboard index page with health and version information.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        var viewModel = new DashboardViewModel();

        try
        {
            // Get health status
            var health = await _apiService.GetHealthAsync();
            if (health != null)
            {
                viewModel.HealthStatus = health.Status ?? "Offline";
                viewModel.HealthResponse = health;
            }
            else
            {
                viewModel.HealthStatus = "Offline";
            }

            // Get version information
            var version = await _apiService.GetVersionAsync();
            if (version != null)
            {
                viewModel.Version = version.Version ?? string.Empty;
                viewModel.Framework = version.Framework ?? string.Empty;
                viewModel.VersionResponse = version;
            }

            // Get system status
            var systemStatus = await _apiService.GetSystemStatusAsync();
            if (systemStatus != null)
            {
                viewModel.SystemStatus = systemStatus;
            }

            // Get repository metrics for dashboard
            var repositories = await _repositoryService.GetRepositoriesAsync();
            viewModel.TotalRepositories = repositories.Count;
            var active = repositories.FirstOrDefault(r => r.IsActive);
            viewModel.ActiveRepositoryName = active?.Name ?? "None";

            if (active != null)
            {
                var summary = await _apiService.GetRepositorySummaryAsync(active.Id);
                if (summary != null)
                {
                    viewModel.TotalFiles = summary.TotalFiles;
                    viewModel.TotalProjects = summary.ProjectCount;
                    viewModel.TotalFolders = summary.TotalFolders;
                    viewModel.TotalLanguages = summary.LanguageCount;
                    viewModel.TotalRepositorySize = summary.TotalSizeFormatted;
                    viewModel.LastScanDate = summary.LastScanDate?.ToString("g") ?? "Never";
                }
            }

            viewModel.IsLoaded = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard data");
            viewModel.HealthStatus = "Offline";
            viewModel.IsLoaded = true;
        }

        return View(viewModel);
    }
}
