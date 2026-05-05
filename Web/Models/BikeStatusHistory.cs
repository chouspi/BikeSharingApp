namespace Web.Models;
public class BikeStatusHistory
{
    public int Id { get; set; }


    public int BikeId { get; set; }
    public Bike Bike { get; set; } = null!;
    public BikeStatus NewStatus { get; set; }


    public int? StationId { get; set; }
    public Station? Station { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public int? RentalId { get; set; }
    public Rental? Rental { get; set; }
}
