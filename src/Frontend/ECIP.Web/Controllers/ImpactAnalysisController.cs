using Microsoft.AspNetCore.Mvc;

namespace ECIP.Web.Controllers;

public class ImpactAnalysisController : Controller
{
    public IActionResult Index()
    {
        return View("_Placeholder", "Change Impact Analysis");
    }
}
