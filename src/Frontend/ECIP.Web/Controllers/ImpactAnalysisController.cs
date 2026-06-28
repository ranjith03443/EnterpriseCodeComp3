using ECIP.Core.Interfaces;
using ECIP.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECIP.Web.Controllers;

public class ImpactAnalysisController : Controller
{
    private readonly IApiService _api;
    private readonly IRepositoryService _repositories;

    public ImpactAnalysisController(IApiService api, IRepositoryService repositories)
    {
        _api = api;
        _repositories = repositories;
    }

    public async Task<IActionResult> Index(Guid? repositoryId)
    {
        ViewBag.Repositories = await _repositories.GetRepositoriesAsync();
        ViewBag.SelectedId   = repositoryId;
        ViewBag.AiAvailable  = await _api.IsAiServiceAvailableAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Analyze([FromBody] ImpactAnalysisRequest request)
    {
        if (request.RepositoryId == Guid.Empty || string.IsNullOrWhiteSpace(request.Component))
            return BadRequest(new { error = "RepositoryId and Component are required." });

        var result = await _api.GetImpactAnalysisAsync(request.RepositoryId, request.Component, request.ChangeType);
        if (result is null)
            return StatusCode(503, new { error = "Analysis service unavailable." });

        return Ok(result);
    }
}

public class ImpactAnalysisRequest
{
    public Guid RepositoryId { get; set; }
    public string Component  { get; set; } = string.Empty;
    public string ChangeType { get; set; } = "modify";
}
