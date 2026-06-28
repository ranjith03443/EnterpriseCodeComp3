using ECIP.Core.Interfaces;
using ECIP.Shared.DTOs.AI;
using ECIP.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECIP.Web.Controllers;

public class FlowExplorerController : Controller
{
    private readonly IApiService _api;
    private readonly IRepositoryService _repositories;
    private readonly ILogger<FlowExplorerController> _logger;

    public FlowExplorerController(IApiService api, IRepositoryService repositories, ILogger<FlowExplorerController> logger)
    {
        _api = api;
        _repositories = repositories;
        _logger = logger;
    }

    public async Task<IActionResult> Index(Guid? repositoryId)
    {
        ViewBag.Repositories = await _repositories.GetRepositoriesAsync();
        ViewBag.SelectedId   = repositoryId;

        if (repositoryId.HasValue)
        {
            ViewBag.FlowView   = await _api.GetFlowsAsync(repositoryId.Value);
            ViewBag.AiAvailable = await _api.IsAiServiceAvailableAsync();
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ExplainFlow([FromBody] ExplainFlowRequest request)
    {
        var result = await _api.AskPlannerAsync(new PlannerRequestDto
        {
            Query   = $"Explain the execution flow for: {request.FlowName}",
            Context = request.Context,
        });

        if (result is null)
            return StatusCode(503, new { error = "AI service unavailable." });

        return Ok(result);
    }
}

public class ExplainFlowRequest
{
    public string FlowName { get; set; } = string.Empty;
    public string Context  { get; set; } = string.Empty;
}
