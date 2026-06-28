using ECIP.Core.Interfaces;
using ECIP.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECIP.Web.Controllers;

public class OnboardingController : Controller
{
    private readonly IApiService _api;
    private readonly IRepositoryService _repositories;

    public OnboardingController(IApiService api, IRepositoryService repositories)
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
    public async Task<IActionResult> Ask([FromBody] OnboardingAskRequest request)
    {
        if (request.RepositoryId == Guid.Empty || string.IsNullOrWhiteSpace(request.Query))
            return BadRequest(new { error = "RepositoryId and Query are required." });

        var result = await _api.AskOnboardingAsync(request.RepositoryId, request.Query);
        if (result is null)
            return StatusCode(503, new { error = "AI service unavailable." });

        return Ok(result);
    }
}

public class OnboardingAskRequest
{
    public Guid RepositoryId { get; set; }
    public string Query { get; set; } = string.Empty;
}
