using Microsoft.AspNetCore.Mvc;

namespace ECIP.Web.Controllers;

public class CodeGeneratorController : Controller
{
    public IActionResult Index()
    {
        return View("_Placeholder", "AI Code Generator");
    }
}
