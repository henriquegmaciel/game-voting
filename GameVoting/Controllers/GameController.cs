using System.Security.Claims;
using GameVoting.Models.Entities;
using GameVoting.Models.ViewModels;
using GameVoting.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameVoting.Controllers;

[Authorize]
public class GameController : Controller
{
    private readonly IGameService _gameService;
    public GameController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [AllowAnonymous]
    public IActionResult Index()
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        GameIndexViewModel viewModel = _gameService.GetRanking(userId);
        return View(viewModel);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(CreateGameViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        _gameService.AddGame(new Game
        {
            Title = model.Title,
            Genre = model.Genre,
            ReleaseYear = model.ReleaseYear,
            ImageUrl = model.ImageUrl,
            StorePageUrl = model.StorePageUrl,
            AddedAt = DateTime.UtcNow
        });

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Remove(int gameId)
    {
        _gameService.RemoveGame(gameId);
        return RedirectToAction("Index", "Game");
    }
}
