namespace GameVoting.Models.Entities;

public class Game
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    public string? Genre { get; set; }
    public int? ReleaseYear { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public ICollection<GameSession> Sessions { get; set; } = new List<GameSession>();
    public string? StorePageUrl { get; set; }
}
