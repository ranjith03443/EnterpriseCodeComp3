using Microsoft.AspNetCore.Mvc;
using ECIP.Web.Services;

namespace ECIP.Web.Controllers;

public class PromptRegistryController : Controller
{
    private readonly IApiService _api;

    public PromptRegistryController(IApiService api)
    {
        _api = api;
    }

    public async Task<IActionResult> Index()
    {
        var prompts = await _api.GetPromptsAsync() ?? new();
        return View(prompts);
    }

    public async Task<IActionResult> Detail(string name)
    {
        var prompt = await _api.GetPromptAsync(name);
        if (prompt is null) return NotFound();
        return View(prompt);
    }
}
