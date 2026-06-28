using ECIP.Core.Interfaces;
using ECIP.Shared.DTOs.AI;
using ECIP.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECIP.Web.Controllers;

public class ArchitectureController : Controller
{
    private readonly IApiService _api;
    private readonly IRepositoryService _repositories;
    private readonly ILogger<ArchitectureController> _logger;

    public ArchitectureController(IApiService api, IRepositoryService repositories, ILogger<ArchitectureController> logger)
    {
        _api = api;
        _repositories = repositories;
        _logger = logger;
    }

    public async Task<IActionResult> Index(Guid? repositoryId)
    {
        var repos = await _repositories.GetRepositoriesAsync();
        ViewBag.Repositories = repos;
        ViewBag.SelectedId = repositoryId;

        if (repositoryId.HasValue)
        {
            ViewBag.Architecture = await _api.GetArchitectureAsync(repositoryId.Value);
            ViewBag.AiAvailable = await _api.IsAiServiceAvailableAsync();
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Analyse([FromBody] ArchitectureAnalysisRequest request)
    {
        var result = await _api.AskPlannerAsync(new PlannerRequestDto
        {
            Query = "Analyse the architecture and explain the layers, patterns, and design decisions in this codebase.",
            Context = request.Context,
        });

        if (result is null)
            return StatusCode(503, new { error = "AI service unavailable." });

        return Ok(result);
    }
}

public class ArchitectureAnalysisRequest
{
    public string Context { get; set; } = string.Empty;
}
