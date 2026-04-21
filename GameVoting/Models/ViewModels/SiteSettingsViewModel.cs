using System.ComponentModel.DataAnnotations;

namespace GameVoting.Models.ViewModels;

public class SiteSettingsViewModel
{
    [Required(ErrorMessage = "Tamanho de lista principal obrigatório.")]
    [Range(1, 100, ErrorMessage = "Entre 1 e 100")]
    public int? MainListSize { get; set; }

    [Required(ErrorMessage = "Tamanho de lista mais votados obrigatório.")]
    [Range(1, 100, ErrorMessage = "Entre 1 e 100")]
    public int? TopListSize { get; set; }

    [Required(ErrorMessage = "Tamanho do histórico de sessões obrigatório.")]
    [Range(1, 100, ErrorMessage = "Entre 1 e 100")]
    public int? HistoryListSize { get; set; }

    [Required(ErrorMessage = "Tamanho da lista de agendamentos obrigatório.")]
    [Range(1, 100, ErrorMessage = "Entre 1 e 100")]
    public int? ScheduleListSize { get; set; }
}
