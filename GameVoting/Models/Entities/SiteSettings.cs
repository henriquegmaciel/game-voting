namespace GameVoting.Models.Entities;

public class SiteSettings
{
    public int Id { get; set; }
    public int MainListSize { get; set; } = 20;
    public int TopListSize { get; set; } = 20;
    public int HistoryListSize { get; set; } = 10;
    public int ScheduleListSize { get; set; } = 10;
}
