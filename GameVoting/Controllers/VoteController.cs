using GameVoting.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameVoting.Controllers;

[Authorize]
public class VoteController : Controller
{
    private readonly IVoteService _voteService;

    public VoteController(IVoteService voteService)
    {
        _voteService = voteService;
    }

    [HttpPost]
    public IActionResult Cast(int gameId)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        bool voted = _voteService.Vote(userId, gameId);
        if (!voted)
        {
            TempData["Erro"] = "Você já votou neste jogo.";
        }

        return RedirectToAction("Index", "Game");
    }

    [HttpPost]
    public IActionResult Remove(int gameId)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        _voteService.RemoveVote(userId, gameId);
        return RedirectToAction("Index", "Game");
    }
}
