using Microsoft.AspNetCore.Mvc;

namespace ECIP.Web.Controllers;

public class OnboardingController : Controller
{
    public IActionResult Index()
    {
        return View("_Placeholder", "Onboarding Assistant");
    }
}
