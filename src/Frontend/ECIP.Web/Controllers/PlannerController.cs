using ECIP.Shared.DTOs.AI;
using ECIP.Web.Services;
using ECIP.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ECIP.Web.Controllers;

public class PlannerController : Controller
{
    private readonly IApiService _api;
    private readonly ILogger<PlannerController> _logger;

    public PlannerController(IApiService api, ILogger<PlannerController> logger)
    {
        _api = api;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var vm = new PlannerViewModel
        {
            AiServiceAvailable = await _api.IsAiServiceAvailableAsync()
        };
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Ask([FromBody] PlannerRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
            return BadRequest(new { error = "Query is required." });

        _logger.LogInformation("Planner Ask: {Query}", request.Query);
        var result = await _api.AskPlannerAsync(request);

        if (result is null)
            return StatusCode(503, new { error = "AI service is unavailable. Ensure ECIP.AIService is running on port 8000." });

        return Ok(result);
    }
}
