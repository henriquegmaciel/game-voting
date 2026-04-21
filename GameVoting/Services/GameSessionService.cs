using GameVoting.Models.Entities;
using GameVoting.Repositories.Interfaces;
using GameVoting.Services.Interfaces;

namespace GameVoting.Services;

public class GameSessionService : IGameSessionService
{
    private readonly IGameSessionRepository _gameSessionRepository;
    private readonly IVoteRepository _voteRepository;
    private readonly IGameRepository _gameRepository;

    public GameSessionService(
        IGameSessionRepository gameSessionRepository,
        IVoteRepository voteRepository,
        IGameRepository gameRepository)
    {
        _gameSessionRepository = gameSessionRepository;
        _voteRepository = voteRepository;
        _gameRepository = gameRepository;
    }

    public void Schedule(int gameId, DateTime date)
    {
        Game? game = _gameRepository.GetById(gameId);

        _gameSessionRepository.Add(new GameSession
        {
            GameId = gameId,
            ScheduledDate = date,
            GameTitle = game?.Title
        });
    }

    public void MarkAsPlayed(int sessionId, DateTime playedAt)
    {
        GameSession? session = _gameSessionRepository.GetById(sessionId);
        if (session is null) return;

        session.PlayedAt = playedAt;
        _gameSessionRepository.Update(session);

        if (session.GameId.HasValue)
            _voteRepository.RemoveAllByGame(session.GameId.Value);
    }

    public void Reschedule(int sessionId, DateTime newDate)
    {
        GameSession? session = _gameSessionRepository.GetById(sessionId);
        if (session is null) return;

        session.ScheduledDate = newDate;
        _gameSessionRepository.Update(session);
    }
    public void CancelSession(int sessionId)
    {
        GameSession? session = _gameSessionRepository.GetById(sessionId);
        if (session is null) return;

        _gameSessionRepository.Remove(session);
    }
}
