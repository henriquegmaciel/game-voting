using GameVoting.Models.Entities;
using GameVoting.Repositories.Interfaces;
using GameVoting.Services;
using Moq;

namespace GameVoting.Tests.Services;

public class GameSessionServiceTests
{
    private readonly Mock<IGameSessionRepository> _gameSessionRepositoryMock;
    private readonly Mock<IVoteRepository> _voteRepositoryMock;
    private readonly Mock<IGameRepository> _gameRepositoryMock;
    private readonly GameSessionService _gameSessionService;

    public GameSessionServiceTests()
    {
        _gameSessionRepositoryMock = new Mock<IGameSessionRepository>();
        _voteRepositoryMock = new Mock<IVoteRepository>();
        _gameRepositoryMock = new Mock<IGameRepository>();

        _gameSessionService = new GameSessionService(
            _gameSessionRepositoryMock.Object,
            _voteRepositoryMock.Object,
            _gameRepositoryMock.Object);
    }

    [Fact]
    public void Schedule_ShouldScheduleWithCorrectDate()
    {
        var date = new DateTime(2024, 6, 15);
        _gameRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns(new Game { Title = "Elden Ring" });

        _gameSessionService.Schedule(1, date);

        _gameSessionRepositoryMock.Verify(r =>
            r.Add(It.Is<GameSession>(s => s.ScheduledDate == date)), Times.Once);
    }

    [Fact]
    public void Schedule_WhenGameExists_ShouldScheduleWithGameTitle()
    {
        string gameTitle = "Elden Ring";
        _gameRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns(new Game { Title = gameTitle });

        _gameSessionService.Schedule(1, new DateTime(2024, 6, 15));

        _gameSessionRepositoryMock.Verify(r =>
            r.Add(It.Is<GameSession>(s => s.GameTitle == gameTitle)), Times.Once);
    }

    [Fact]
    public void Schedule_WhenGameDoesntExist_ShouldScheduleWithNullGameTitle()
    {
        _gameRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns((Game?)null);

        _gameSessionService.Schedule(1, new DateTime(2024, 6, 15));

        _gameSessionRepositoryMock.Verify(r =>
            r.Add(It.Is<GameSession>(s => s.GameTitle == null)), Times.Once);
    }

    [Fact]
    public void MarkAsPlayed_WhenSessionExists_ShouldFillPlayedAt()
    {
        var playedAt = new DateTime(2024, 6, 15);
        _gameSessionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns(new GameSession());

        _gameSessionService.MarkAsPlayed(1, playedAt);

        _gameSessionRepositoryMock.Verify(r =>
            r.Update(It.Is<GameSession>(s => s.PlayedAt == playedAt)), Times.Once);
    }

    [Fact]
    public void MarkAsPlayed_WhenGameExists_ShouldResetVotes()
    {
        int gameId = 1;
        _gameSessionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns(new GameSession { GameId = gameId });

        _gameSessionService.MarkAsPlayed(gameId, DateTime.UtcNow);

        _voteRepositoryMock.Verify(r => r.RemoveAllByGame(gameId), Times.Once);
    }

    [Fact]
    public void MarkAsPlayed_WhenSessionDoesntExist_ShouldDoNothing()
    {
        _gameSessionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns((GameSession?)null);

        _gameSessionService.MarkAsPlayed(1, DateTime.UtcNow);

        _gameSessionRepositoryMock.Verify(r => r.Update(It.IsAny<GameSession>()), Times.Never);
    }

    [Fact]
    public void MarkAsPlayed_WhenGameDoesntExist_ShouldNotTryToResetVotes()
    {
        _gameSessionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns(new GameSession());

        _gameSessionService.MarkAsPlayed(1, DateTime.UtcNow);

        _voteRepositoryMock.Verify(r => r.RemoveAllByGame(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void CancelSession_WhenSessionExists_ShouldRemoveSession()
    {
        var session = new GameSession() { Id = 1, ScheduledDate = new DateTime(2025, 2, 5) };
        _gameSessionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns(session);

        _gameSessionService.CancelSession(1);

        _gameSessionRepositoryMock.Verify(r => r.Remove(session), Times.Once);
    }

    [Fact]
    public void CancelSession_WhenSessionDoesntExist_ShouldDoNothing()
    {
        _gameSessionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns((GameSession?)null);

        _gameSessionService.CancelSession(1);

        _gameSessionRepositoryMock.Verify(r => r.Remove(It.IsAny<GameSession>()), Times.Never);
    }

    [Fact]
    public void Reschedule_WhenSessionExists_ShouldUpdateScheduledDate()
    {
        DateTime newDate = new(2025, 1, 1);
        _gameSessionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns(new GameSession());

        _gameSessionService.Reschedule(1, newDate);

        _gameSessionRepositoryMock.Verify(r =>
            r.Update(It.Is<GameSession>(s => s.ScheduledDate == newDate)), Times.Once);
    }

    [Fact]
    public void Reschedule_WhenSessionDoesntExist_ShouldDoNothing()
    {
        _gameSessionRepositoryMock
            .Setup(r => r.GetById(1))
            .Returns((GameSession?)null);

        _gameSessionService.Reschedule(1, DateTime.UtcNow);

        _gameSessionRepositoryMock.Verify(r => r.Update(It.IsAny<GameSession>()), Times.Never);
    }
}
