using GameVoting.Models.Entities;
using GameVoting.Models.ViewModels;
using GameVoting.Repositories.Interfaces;
using GameVoting.Services.Interfaces;

namespace GameVoting.Services;

public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;
    private readonly IGameSessionRepository _sessionRepository;
    private readonly ISiteSettingsRepository _settingsRepository;
    private readonly IVoteRepository _voteRepository;

    public GameService(
        IGameRepository gameRepository,
        IGameSessionRepository sessionRepository,
        ISiteSettingsRepository settingsRepository,
        IVoteRepository voteRepository)
    {
        _gameRepository = gameRepository;
        _sessionRepository = sessionRepository;
        _settingsRepository = settingsRepository;
        _voteRepository = voteRepository;
    }

    public GameIndexViewModel GetRanking(string? userId)
    {
        List<GameSession> upcoming = _sessionRepository.GetUpcoming().ToList();
        HashSet<int> scheduledGameIds = upcoming
            .Where(s => s.GameId.HasValue)
            .Select(s => s.GameId!.Value)
            .ToHashSet();
        List<Game> games = _gameRepository.GetAll()
            .Where(g => !scheduledGameIds.Contains(g.Id))
            .ToList();
        SiteSettings settings = _settingsRepository.Get();

        HashSet<int> userVotedGameIds = userId is not null
            ? _voteRepository.GetVotedGameIdsByUser(userId)
            : new HashSet<int>();

        var toRankingItem = (Game g) => new RankingItemViewModel
        {
            GameId = g.Id,
            Title = g.Title,
            Genre = g.Genre,
            ReleaseYear = g.ReleaseYear,
            ImageUrl = g.ImageUrl,
            StorePageUrl = g.StorePageUrl,
            VoteCount = g.Votes.Count,
            UserAlreadyVoted = userVotedGameIds.Contains(g.Id)
        };

        return new GameIndexViewModel
        {
            Ranking = games
                .OrderByDescending(g => g.Votes.Count != 0
                    ? g.Votes.Max(v => v.VotedAt)
                    : DateTime.MinValue)
                .Take(settings.MainListSize)
                .Select(toRankingItem),
            TopRanking = games
                .OrderByDescending(g => g.Votes.Count)
                .ThenBy(g => g.Votes.Count != 0
                    ? g.Votes.Max(v => v.VotedAt)
                    : DateTime.MaxValue)
                .Take(settings.TopListSize)
                .Select(toRankingItem),
            Upcoming = upcoming
                .Take(settings.ScheduleListSize),
            History = _sessionRepository.GetHistory()
                .Take(settings.HistoryListSize)
        };
    }

    public void AddGame(Game game)
    {
        _gameRepository.Add(game);
    }

    public void RemoveGame(int gameId)
    {
        Game? game = _gameRepository.GetById(gameId);
        if (game is not null)
            _gameRepository.Remove(game);
    }
}
