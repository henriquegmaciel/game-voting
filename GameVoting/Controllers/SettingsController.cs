using GameVoting.Models.Entities;
using GameVoting.Models.ViewModels;
using GameVoting.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameVoting.Controllers;

[Authorize(Roles = "Admin")]
public class SettingsController : Controller
{
    private readonly ISiteSettingsRepository _settingsRepository;

    public SettingsController(ISiteSettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }

    public IActionResult Index()
    {
        SiteSettings settings = _settingsRepository.Get();
        return View(new SiteSettingsViewModel
        {
            MainListSize = settings.MainListSize,
            TopListSize = settings.TopListSize,
            HistoryListSize = settings.HistoryListSize,
            ScheduleListSize = settings.ScheduleListSize
        });
    }

    [HttpPost]
    public IActionResult Index(SiteSettingsViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        SiteSettings settings = _settingsRepository.Get();
        settings.MainListSize = model.MainListSize!.Value;
        settings.TopListSize = model.TopListSize!.Value;
        settings.HistoryListSize = model.HistoryListSize!.Value;
        settings.ScheduleListSize = model.ScheduleListSize!.Value;

        _settingsRepository.Update(settings);
        TempData["Sucesso"] = "Configurações salvas!";
        return RedirectToAction("Index", "Game");
    }
}
