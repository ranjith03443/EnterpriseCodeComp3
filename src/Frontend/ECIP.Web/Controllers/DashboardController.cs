using Microsoft.AspNetCore.Mvc;
using ECIP.Core.Interfaces;
using ECIP.Web.Services;
using ECIP.Web.ViewModels;

namespace ECIP.Web.Controllers;

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

    public async Task<IActionResult> Index()
    {
        var viewModel = new DashboardViewModel();

        try
        {
            // Fire all independent calls in parallel
            var healthTask      = _apiService.GetHealthAsync();
            var versionTask     = _apiService.GetVersionAsync();
            var systemTask      = _apiService.GetSystemStatusAsync();
            var reposTask       = _repositoryService.GetRepositoriesAsync();

            await Task.WhenAll(healthTask, versionTask, systemTask, reposTask);

            var health   = healthTask.Result;
            var version  = versionTask.Result;
            var system   = systemTask.Result;
            var repos    = reposTask.Result;

            viewModel.HealthStatus   = health?.Status ?? "Offline";
            viewModel.HealthResponse = health;

            if (version is not null)
            {
                viewModel.Version         = version.Version ?? string.Empty;
                viewModel.Framework       = version.Framework ?? string.Empty;
                viewModel.VersionResponse = version;
            }

            if (system is not null)
                viewModel.SystemStatus = system;

            viewModel.TotalRepositories    = repos.Count;
            var active                     = repos.FirstOrDefault(r => r.IsActive);
            viewModel.ActiveRepositoryName = active?.Name ?? "None";

            if (active is not null)
            {
                var summary = await _apiService.GetRepositorySummaryAsync(active.Id);
                if (summary is not null)
                {
                    viewModel.TotalFiles           = summary.TotalFiles;
                    viewModel.TotalProjects        = summary.ProjectCount;
                    viewModel.TotalFolders         = summary.TotalFolders;
                    viewModel.TotalLanguages       = summary.LanguageCount;
                    viewModel.TotalRepositorySize  = summary.TotalSizeFormatted;
                    viewModel.LastScanDate         = summary.LastScanDate?.ToString("g") ?? "Never";
                }
            }

            viewModel.IsLoaded = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard data");
            viewModel.HealthStatus = "Offline";
            viewModel.IsLoaded     = true;
        }

        return View(viewModel);
    }
}
