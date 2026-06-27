using ECIP.Core.Entities;
using ECIP.Core.Enums;
using ECIP.Core.Interfaces;
using ECIP.Web.Models;
using ECIP.Web.Services;
using ECIP.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ECIP.Web.Controllers;

/// <summary>
/// Handles repository workspace management views and actions.
/// </summary>
public class RepositoryController : Controller
{
    private readonly IRepositoryService _repositoryService;
    private readonly IApiService _apiService;
    private readonly ILogger<RepositoryController> _logger;

    public RepositoryController(IRepositoryService repositoryService, IApiService apiService, ILogger<RepositoryController> logger)
    {
        _repositoryService = repositoryService;
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var repositories = await _repositoryService.GetRepositoriesAsync();
        var model = new RepositoryWorkspaceViewModel
        {
            Repositories = repositories.ToList()
        };
        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new RepositoryFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RepositoryFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (await HasDuplicateNameAsync(model.Name))
        {
            ModelState.AddModelError(nameof(model.Name), "A repository with this name already exists.");
            return View(model);
        }

        var entity = new RepositoryEntity
        {
            Name = model.Name,
            Description = model.Description,
            RepositoryUrl = model.RepositoryUrl,
            RepositoryType = model.RepositoryType,
            Branch = model.Branch,
            LocalPath = model.LocalPath,
            DefaultBranch = model.DefaultBranch,
            Status = model.Status,
            IsActive = model.IsActive
        };

        await _repositoryService.AddRepositoryAsync(entity);
        _logger.LogInformation("Repository created via UI: {Name}", model.Name);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var repository = await _repositoryService.GetRepositoryAsync(id);
        if (repository is null)
        {
            return NotFound();
        }

        var model = new RepositoryFormViewModel
        {
            Id = repository.Id,
            Name = repository.Name,
            Description = repository.Description,
            RepositoryUrl = repository.RepositoryUrl,
            RepositoryType = repository.RepositoryType,
            Branch = repository.Branch,
            LocalPath = repository.LocalPath,
            DefaultBranch = repository.DefaultBranch,
            Status = repository.Status,
            IsActive = repository.IsActive
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(RepositoryFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var existing = await _repositoryService.GetRepositoryAsync(model.Id);
        if (existing is null)
        {
            return NotFound();
        }

        if (await HasDuplicateNameAsync(model.Name, model.Id))
        {
            ModelState.AddModelError(nameof(model.Name), "A repository with this name already exists.");
            return View(model);
        }

        var updated = new RepositoryEntity
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            RepositoryUrl = model.RepositoryUrl,
            RepositoryType = model.RepositoryType,
            Branch = model.Branch,
            LocalPath = model.LocalPath,
            DefaultBranch = model.DefaultBranch,
            Status = model.Status,
            IsActive = model.IsActive
        };

        await _repositoryService.UpdateRepositoryAsync(updated);
        _logger.LogInformation("Repository updated via UI: {Name}", model.Name);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, string tab = "summary")
    {
        var repository = await _repositoryService.GetRepositoryAsync(id);
        if (repository is null)
        {
            return NotFound();
        }

        var model = new RepositoryDetailsViewModel
        {
            Repository = repository,
            ActiveTab = tab
        };

        // Load metadata from API
        var summary = await _apiService.GetRepositorySummaryAsync(id);
        if (summary != null)
        {
            model.Summary = summary;
        }

        var statistics = await _apiService.GetRepositoryStatisticsAsync(id);
        if (statistics != null)
        {
            model.Statistics = statistics;
            model.Languages = statistics.Languages;
            model.Projects = statistics.Projects;
            model.FolderStatistics = statistics.FolderStatistics;
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Scan(Guid id)
    {
        var success = await _apiService.ScanRepositoryAsync(id);
        if (success)
        {
            TempData["SuccessMessage"] = "Repository scan completed successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = "Repository scan failed. Ensure the local path is valid.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _repositoryService.DeleteRepositoryAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Activate(Guid id)
    {
        await _repositoryService.SetActiveRepositoryAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Clone()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Clone(string repositoryUrl, string branch, string localPath)
    {
        try
        {
            await _repositoryService.CloneRepositoryAsync(repositoryUrl, localPath, branch);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Clone failed for {RepositoryUrl}", repositoryUrl);
            TempData["ErrorMessage"] = "Repository clone failed. Please verify the URL and authentication details.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public IActionResult OpenLocal()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OpenLocal(string name, string localPath, string description)
    {
        try
        {
            await _repositoryService.RegisterLocalRepositoryAsync(name, localPath, description);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Local repository registration failed for {LocalPath}", localPath);
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    private async Task<bool> HasDuplicateNameAsync(string name, Guid? ignoreId = null)
    {
        var repositories = await _repositoryService.GetRepositoriesAsync();
        return repositories.Any(r => string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase) && (!ignoreId.HasValue || r.Id != ignoreId.Value));
    }
}
