using Microsoft.AspNetCore.Mvc;

namespace ECIP.Web.Controllers;

public class PromptRegistryController : Controller
{
    public IActionResult Index()
    {
        return View("_Placeholder", "Prompt Registry");
    }
}
