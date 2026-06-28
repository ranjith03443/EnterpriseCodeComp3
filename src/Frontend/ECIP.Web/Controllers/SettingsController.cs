using Microsoft.AspNetCore.Mvc;
using ECIP.Web.Services;
using ECIP.Web.ViewModels;

namespace ECIP.Web.Controllers;

public class SettingsController : Controller
{
    private readonly IApiService _api;
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(IApiService api, ILogger<SettingsController> logger)
    {
        _api = api;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var dto = await _api.GetAiSettingsAsync();
        var vm = dto is not null
            ? AiSettingsViewModel.FromDto(dto)
            : new AiSettingsViewModel { AiServiceAvailable = false };
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Save(AiSettingsViewModel vm)
    {
        var dto = vm.ToDto();
        var result = await _api.SaveAiSettingsAsync(dto);
        if (result is null)
        {
            vm.AiServiceAvailable = false;
            vm.SaveMessage = "Failed to save settings — AI service is unavailable.";
            vm.SaveSuccess = false;
        }
        else
        {
            vm = AiSettingsViewModel.FromDto(result);
            vm.SaveMessage = "Settings saved successfully.";
            vm.SaveSuccess = true;
        }
        return View("Index", vm);
    }

    [HttpPost]
    public async Task<IActionResult> TestConnection()
    {
        var results = await _api.TestAiConnectionAsync();
        if (results is null)
            return Json(new { success = false, message = "AI service is unavailable.", results = Array.Empty<object>() });
        return Json(new { success = true, results });
    }
}
