using Microsoft.AspNetCore.Mvc;

namespace ECIP.Web.Controllers;

public class AboutController : Controller
{
    public IActionResult Index()
    {
        return View("_Placeholder", "About");
    }
}
