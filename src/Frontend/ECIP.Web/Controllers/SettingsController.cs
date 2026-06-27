using Microsoft.AspNetCore.Mvc;

namespace ECIP.Web.Controllers;

public class SettingsController : Controller
{
    public IActionResult Index()
    {
        return View("_Placeholder", "Settings");
    }
}
