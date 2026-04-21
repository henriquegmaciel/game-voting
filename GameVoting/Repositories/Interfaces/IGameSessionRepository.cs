using GameVoting.Models.Entities;

namespace GameVoting.Repositories.Interfaces;

public interface IGameSessionRepository
{
    IEnumerable<GameSession> GetUpcoming();
    IEnumerable<GameSession> GetHistory();
    GameSession? GetById(int id);
    void Add(GameSession session);
    void Update(GameSession session);
    void Remove(GameSession session);
}
