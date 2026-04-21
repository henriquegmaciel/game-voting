using GameVoting.Models.Entities;
using GameVoting.Models.ViewModels;

namespace GameVoting.Services.Interfaces;

public interface IGameService
{
    GameIndexViewModel GetRanking(string? userId);
    void AddGame(Game game);
    void RemoveGame(int gameId);
}
