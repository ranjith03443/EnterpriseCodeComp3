using Microsoft.AspNetCore.Mvc;

namespace ECIP.Web.Controllers;

public class FlowExplorerController : Controller
{
    public IActionResult Index()
    {
        return View("_Placeholder", "Flow Explorer");
    }
}
