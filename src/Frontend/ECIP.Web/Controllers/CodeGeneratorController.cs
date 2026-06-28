using ECIP.Core.Interfaces;
using ECIP.Shared.DTOs.AI;
using ECIP.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECIP.Web.Controllers;

public class CodeGeneratorController : Controller
{
    private readonly IApiService _api;
    private readonly IRepositoryService _repositories;

    public CodeGeneratorController(IApiService api, IRepositoryService repositories)
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
    public async Task<IActionResult> Generate([FromBody] CodeGenerationRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Template))
            return BadRequest(new { error = "Name and Template are required." });

        var result = await _api.GenerateCodeAsync(request);
        if (result is null)
            return StatusCode(503, new { error = "AI service unavailable." });

        return Ok(result);
    }
}
