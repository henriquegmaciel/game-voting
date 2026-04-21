using GameVoting.Models.Entities;
using GameVoting.Repositories.Interfaces;
using GameVoting.Services;
using Moq;

namespace GameVoting.Tests.Services;

public class GameServiceTests
{
    private readonly Mock<IGameRepository> _gameRepositoryMock;
    private readonly Mock<IGameSessionRepository> _gameSessionRepositoryMock;
    private readonly Mock<ISiteSettingsRepository> _settingsRepositoryMock;
    private readonly Mock<IVoteRepository> _voteRepositoryMock;
    private readonly GameService _gameService;

    public GameServiceTests()
    {
        _gameRepositoryMock = new Mock<IGameRepository>();
        _gameSessionRepositoryMock = new Mock<IGameSessionRepository>();
        _settingsRepositoryMock = new Mock<ISiteSettingsRepository>();
        _voteRepositoryMock = new Mock<IVoteRepository>();

        _gameService = new GameService(
            _gameRepositoryMock.Object,
            _gameSessionRepositoryMock.Object,
            _settingsRepositoryMock.Object,
            _voteRepositoryMock.Object);
    }

    [Fact]
    public void GetRanking_ScheduledGames_ShouldNotAppearInRanking()
    {
        var game = new Game { Id = 1, Title = "Elden Ring", Votes = new List<Vote>() };
        ConfigureDefaultMocks(
            games: new List<Game> { game },
            upcoming: new List<GameSession> { new GameSession { GameId = 1 } });

        var result = _gameService.GetRanking("user1");

        Assert.Empty(result.Ranking);
    }

    [Fact]
    public void GetRanking_ShouldRespectListSizeLimit()
    {
        var games = Enumerable.Range(1, 10)
            .Select(i => new Game { Id = i, Title = $"Game {i}", Votes = new List<Vote>() })
            .ToList();
        ConfigureDefaultMocks(
            games: games,
            settings: new SiteSettings { MainListSize = 3 });

        var result = _gameService.GetRanking("user1");

        Assert.Equal(3, result.Ranking.Count());
    }

    [Fact]
    public void GetRanking_WhenUserVoted_ShouldMarkUserAlreadyVoted()
    {
        var game = new Game { Id = 1, Title = "Elden Ring", Votes = new List<Vote>() };
        ConfigureDefaultMocks(games: new List<Game> { game });
        _voteRepositoryMock.Setup(r => r.GetVotedGameIdsByUser("user1")).Returns(new HashSet<int> { 1 });

        var result = _gameService.GetRanking("user1");

        Assert.True(result.Ranking.First().UserAlreadyVoted);
    }

    [Fact]
    public void GetRanking_WhenUserIsNull_ShouldNotCallGetVotedGameIds()
    {
        var game = new Game { Id = 1, Title = "Elden Ring", Votes = new List<Vote>() };
        ConfigureDefaultMocks(games: new List<Game> { game });

        _gameService.GetRanking(null);

        _voteRepositoryMock.Verify(r =>
            r.GetVotedGameIdsByUser(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void GetRanking_WhenUserIsNull_UserAlreadyVotedShouldBeFalse()
    {
        var game = new Game { Id = 1, Title = "Elden Ring", Votes = new List<Vote>() };
        ConfigureDefaultMocks(games: new List<Game> { game });

        var result = _gameService.GetRanking(null);

        Assert.False(result.Ranking.First().UserAlreadyVoted);
    }

    [Fact]
    public void GetRanking_RankingShouldOrderByLastVoteDate()
    {
        var oldVote = new Vote { VotedAt = new DateTime(2024, 1, 1) };
        var newVote = new Vote { VotedAt = new DateTime(2024, 6, 1) };

        var games = new List<Game>
        {
            new Game { Id = 1, Title = "Old Game", Votes = new List<Vote> { oldVote } },
            new Game { Id = 2, Title = "New Game", Votes = new List<Vote> { newVote } }
        };

        ConfigureDefaultMocks(games: games);

        var result = _gameService.GetRanking(null);

        Assert.Equal(2, result.Ranking.First().GameId);
    }

    [Fact]
    public void GetRanking_TopRankingShouldOrderByMostVotes()
    {
        var firstVote = new Vote { VotedAt = new DateTime(2024, 1, 1) };
        var secondVote = new Vote { VotedAt = new DateTime(2024, 6, 1) };
        var thirdVote = new Vote { VotedAt = new DateTime(2024, 10, 1) };

        var games = new List<Game>
        {
            new Game { Id = 1, Title = "Less Votes Game", Votes = new List<Vote> { firstVote } },
            new Game { Id = 2, Title = "More Votes Game", Votes = new List<Vote> { secondVote, thirdVote } }
        };

        ConfigureDefaultMocks(games: games);

        var result = _gameService.GetRanking(null);

        Assert.Equal(2, result.TopRanking.First().GameId);
    }

    [Fact]
    public void GetRanking_WhenGameWithoutVotesExists_GameWithVotesShouldComeFirst()
    {
        var vote = new Vote { VotedAt = new DateTime(2024, 1, 1) };

        var games = new List<Game>
        {
            new Game { Id = 1, Title = "No Votes Game", Votes = new List<Vote>() },
            new Game { Id = 2, Title = "Has Votes Game", Votes = new List<Vote> { vote } }
        };

        ConfigureDefaultMocks(games: games);

        var result = _gameService.GetRanking(null);

        Assert.Equal(2, result.TopRanking.First().GameId);
    }

    private void ConfigureDefaultMocks(
        List<Game>? games = null,
        List<GameSession>? upcoming = null,
        SiteSettings? settings = null)
    {
        _gameRepositoryMock
            .Setup(r => r.GetAll())
            .Returns(games ?? new List<Game>());

        _gameSessionRepositoryMock
            .Setup(r => r.GetUpcoming())
            .Returns(upcoming ?? new List<GameSession>());

        _gameSessionRepositoryMock
            .Setup(r => r.GetHistory())
            .Returns(new List<GameSession>());

        _settingsRepositoryMock
            .Setup(r => r.Get())
            .Returns(settings ?? new SiteSettings());

        _voteRepositoryMock
            .Setup(r => r.GetVotedGameIdsByUser(It.IsAny<string>()))
            .Returns(new HashSet<int>());
    }

    [Fact]
    public void AddGame_ShouldAddCorrectGame()
    {
        var game = new Game() { Title = "Elden Ring" };

        _gameService.AddGame(game);

        _gameRepositoryMock.Verify(r => r.Add(game), Times.Once);
    }

    [Fact]
    public void RemoveGame_WhenGameExists_ShouldRemoveCorrectGame()
    {
        var game = new Game { Id = 1, Title = "Elden Ring" };
        _gameRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns(game);

        _gameService.RemoveGame(1);

        _gameRepositoryMock.Verify(r => r.Remove(game), Times.Once);
    }

    [Fact]
    public void RemoveGame_WhenGameDoesntExist_ShouldDoNothing()
    {
        _gameRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns((Game?)null);

        _gameService.RemoveGame(1);

        _gameRepositoryMock.Verify(r => r.Remove(It.IsAny<Game>()), Times.Never);
    }
}
