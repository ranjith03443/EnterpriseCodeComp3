using Microsoft.AspNetCore.Mvc;

namespace ECIP.Web.Controllers;

public class KnowledgeGraphController : Controller
{
    public IActionResult Index()
    {
        return View("_Placeholder", "Knowledge Graph");
    }
}
