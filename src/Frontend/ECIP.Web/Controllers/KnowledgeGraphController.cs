using ECIP.Core.Interfaces;
using ECIP.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECIP.Web.Controllers;

public class KnowledgeGraphController : Controller
{
    private readonly IApiService _api;
    private readonly IRepositoryService _repositories;
    private readonly ILogger<KnowledgeGraphController> _logger;

    public KnowledgeGraphController(IApiService api, IRepositoryService repositories, ILogger<KnowledgeGraphController> logger)
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
            ViewBag.Graph = await _api.GetKnowledgeGraphAsync(repositoryId.Value);

        return View();
    }
}
