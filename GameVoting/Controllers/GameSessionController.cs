using GameVoting.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameVoting.Controllers;

[Authorize(Roles = "Admin")]
public class GameSessionController : Controller
{
    private readonly IGameSessionService _sessionService;

    public GameSessionController(IGameSessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [HttpPost]
    public IActionResult Schedule(int gameId, DateTime scheduledDate)
    {
        scheduledDate = DateTime.SpecifyKind(scheduledDate, DateTimeKind.Utc);
        if (scheduledDate == default)
        {
            TempData["Erro"] = "Selecione uma data válida.";
            return RedirectToAction("Index", "Game");
        }

        _sessionService.Schedule(gameId, scheduledDate);
        TempData["Sucesso"] = "Jogo agendado!";
        return RedirectToAction("Index", "Game");
    }

    [HttpPost]
    public IActionResult MarkAsPlayed(int sessionId, DateTime playedAt)
    {
        playedAt = DateTime.SpecifyKind(playedAt, DateTimeKind.Utc);
        if (playedAt == default)
            playedAt = DateTime.Today;

        _sessionService.MarkAsPlayed(sessionId, playedAt);
        TempData["Sucesso"] = "Jogo marcado como jogado!";
        return RedirectToAction("Index", "Game");
    }

    [HttpPost]
    public IActionResult Reschedule(int sessionId, DateTime newDate)
    {
        newDate = DateTime.SpecifyKind(newDate, DateTimeKind.Utc);
        if (newDate == default)
        {
            TempData["Erro"] = "Data inválida.";
            return RedirectToAction("Index", "Game");
        }

        _sessionService.Reschedule(sessionId, newDate);
        TempData["Sucesso"] = "Sessão reagendada!";
        return RedirectToAction("Index", "Game");
    }

    [HttpPost]
    public IActionResult CancelSession(int sessionId)
    {
        _sessionService.CancelSession(sessionId);
        TempData["Sucesso"] = "Sessão cancelada. Jogo voltou para a lista.";
        return RedirectToAction("Index", "Game");
    }
}
