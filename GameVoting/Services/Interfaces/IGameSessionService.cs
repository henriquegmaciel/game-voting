using GameVoting.Models.Entities;

namespace GameVoting.Services.Interfaces;

public interface IGameSessionService
{
    void Schedule(int gameId, DateTime date);
    void MarkAsPlayed(int sessionId, DateTime playedAt);
    void Reschedule(int sessionId, DateTime newDate);
    void CancelSession(int sessionId);
}
