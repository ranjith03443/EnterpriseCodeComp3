using Microsoft.AspNetCore.Mvc;

namespace ECIP.Web.Controllers;

public class ArchitectureController : Controller
{
    public IActionResult Index()
    {
        return View("_Placeholder", "Architecture Explorer");
    }
}
