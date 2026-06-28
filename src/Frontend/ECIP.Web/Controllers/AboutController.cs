using ECIP.Core.Interfaces;
using ECIP.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECIP.Web.Controllers;

public class AboutController : Controller
{
    private readonly IApiService _api;
    private readonly IRepositoryService _repositories;

    public AboutController(IApiService api, IRepositoryService repositories)
    {
        _api = api;
        _repositories = repositories;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Health      = await _api.GetHealthAsync();
        ViewBag.Version     = await _api.GetVersionAsync();
        ViewBag.AiAvailable = await _api.IsAiServiceAvailableAsync();
        ViewBag.RepoCount   = (await _repositories.GetRepositoriesAsync()).Count;
        return View();
    }
}
