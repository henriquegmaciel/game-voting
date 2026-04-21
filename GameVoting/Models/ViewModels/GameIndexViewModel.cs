using GameVoting.Models.Entities;

namespace GameVoting.Models.ViewModels;

public class GameIndexViewModel
{
    public IEnumerable<RankingItemViewModel> Ranking { get; set; } = new List<RankingItemViewModel>();
    public IEnumerable<GameSession> Upcoming { get; set; } = new List<GameSession>();
    public IEnumerable<GameSession> History { get; set; } = new List<GameSession>();
    public IEnumerable<RankingItemViewModel> TopRanking { get; set; } = new List<RankingItemViewModel>();
}
