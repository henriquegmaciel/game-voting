using GameVoting.Data;
using GameVoting.Models.Entities;
using GameVoting.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GameVoting.Repositories;

public class GameSessionRepository : IGameSessionRepository
{
    private readonly AppDbContext _context;

    public GameSessionRepository(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<GameSession> GetUpcoming()
    {
        return _context.GameSessions
            .Include(s => s.Game)
            .Where(s => !s.PlayedAt.HasValue)
            .OrderBy(s => s.ScheduledDate)
            .ToList();
    }

    public IEnumerable<GameSession> GetHistory()
    {
        return _context.GameSessions
            .Include(s => s.Game)
            .Where(s => s.PlayedAt.HasValue)
            .OrderByDescending(s => s.PlayedAt)
            .ToList();
    }

    public GameSession? GetById(int id)
    {
        return _context.GameSessions.Find(id);
    }

    public void Add(GameSession session)
    {
        _context.GameSessions.Add(session);
        _context.SaveChanges();
    }

    public void Update(GameSession session)
    {
        _context.GameSessions.Update(session);
        _context.SaveChanges();
    }

    public void Remove(GameSession session)
    {
        _context.GameSessions.Remove(session);
        _context.SaveChanges();
    }
}
