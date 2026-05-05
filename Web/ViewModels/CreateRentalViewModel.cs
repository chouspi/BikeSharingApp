using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.ViewModels;

public class CreateRentalViewModel
{
    public int BikeId { get; set; }
    public int StationId { get; set; }

    public string BikeCode { get; set; } = "";
    public string StationName { get; set; } = "";

    [Required(ErrorMessage = "Vyber odhadovanou cilovou stanici.")]
    public int? EstimatedTargetStationId { get; set; }

    public List<SelectListItem> TargetStationOptions { get; set; } = new();

    public bool AgreeToTerms { get; set; }
}