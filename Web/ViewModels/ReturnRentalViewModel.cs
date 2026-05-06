using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels;

public class ReturnRentalViewModel
{
    public int RentalId { get; set; }
    public string BikeCode { get; set; } = "";
    public string StartStationName { get; set; } = "";
    public DateTime StartedAt { get; set; }

    [Required(ErrorMessage = "Vyber stanici vraceni.")]
    public int? EndStationId { get; set; }

    public List<SelectListItem> StationOptions { get; set; } = new();

    public string? Note { get; set; }

    [Required(ErrorMessage = "Musis potvrdit vraceni.")]
    public bool ConfirmReturn { get; set; }
}
