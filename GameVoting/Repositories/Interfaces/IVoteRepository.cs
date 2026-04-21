using GameVoting.Models.Entities;

namespace GameVoting.Repositories.Interfaces;

public interface IVoteRepository
{
    bool HasVoted(string userId, int gameId);
    Vote? GetByUserAndGame(string userId, int gameId);
    void Add(Vote vote);
    void Remove(Vote vote);
    void RemoveAllByGame(int gameId);
    HashSet<int> GetVotedGameIdsByUser(string userId);
}
