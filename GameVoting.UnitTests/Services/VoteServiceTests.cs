using GameVoting.Models.Entities;
using GameVoting.Repositories.Interfaces;
using GameVoting.Services;
using Moq;

namespace GameVoting.Tests.Services;

public class VoteServiceTests
{
    private readonly Mock<IVoteRepository> _voteRepositoryMock;
    private readonly VoteService _voteService;

    public VoteServiceTests()
    {
        _voteRepositoryMock = new Mock<IVoteRepository>();
        _voteService = new VoteService(_voteRepositoryMock.Object);
    }

    [Fact]
    public void Vote_WhenUserAlreadyVoted_ShouldReturnFalse()
    {
        _voteRepositoryMock
            .Setup(r => r.HasVoted("user1", 1))
            .Returns(true);

        bool result = _voteService.Vote("user1", 1);

        Assert.False(result);
    }

    [Fact]
    public void Vote_WhenUserDidntVote_ShouldReturnTrue()
    {
        _voteRepositoryMock
            .Setup(r => r.HasVoted("user1", 1))
            .Returns(false);

        bool result = _voteService.Vote("user1", 1);

        Assert.True(result);
    }

    [Fact]
    public void Vote_WhenUserDidntVote_ShouldAddVote()
    {
        _voteRepositoryMock
            .Setup(r => r.HasVoted("user1", 1))
            .Returns(false);

        _voteService.Vote("user1", 1);

        _voteRepositoryMock.Verify(r => r.Add(It.IsAny<Vote>()), Times.Once);
    }

    [Fact]
    public void Vote_WhenUserAlreadyVoted_ShoudlNotAddVote()
    {
        _voteRepositoryMock
            .Setup(r => r.HasVoted("user1", 1))
            .Returns(true);

        _voteService.Vote("user1", 1);

        _voteRepositoryMock.Verify(r => r.Add(It.IsAny<Vote>()), Times.Never);
    }

    [Fact]
    public void RemoveVote_WhenVoteDoesntExist_ShouldDoNothing()
    {
        _voteRepositoryMock
            .Setup(r => r.GetByUserAndGame("user1", 1))
            .Returns((Vote?)null);

        _voteService.RemoveVote("user1", 1);

        _voteRepositoryMock.Verify(r => r.Remove(It.IsAny<Vote>()), Times.Never);
    }

    [Fact]
    public void RemoveVote_WhenVoteExists_ShouldRemoveVote()
    {
        var vote = new Vote() { Id = 1, GameId = 1 };
        _voteRepositoryMock
            .Setup(r => r.GetByUserAndGame("user1", 1))
            .Returns(vote);

        _voteService.RemoveVote("user1", 1);

        _voteRepositoryMock.Verify(r => r.Remove(vote), Times.Once);
    }
}
