namespace GameVoting.Models.Entities;

public class GameSession
{
    public int Id { get; set; }
    public int? GameId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime? PlayedAt { get; set; }

    public Game Game { get; set; } = null!;
    public string? GameTitle { get; set; }
}
