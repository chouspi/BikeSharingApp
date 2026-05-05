using System.ComponentModel.DataAnnotations;

namespace Web.Models;
public class Bike
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    public BikeStatus Status { get; set; } = BikeStatus.Available;
    public int? CurrentStationId { get; set; }
    public Station? CurrentStation { get; set; }

    public bool IsActive { get; set; } = true;
    public List<Rental> Rentals { get; set; } = new List<Rental>();
    public List<BikeStatusHistory> StatusHistory { get; set; } = new List<BikeStatusHistory>();
}
