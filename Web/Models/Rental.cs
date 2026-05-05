namespace Web.Models;
public class Rental
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public int BikeId { get; set; }
    public Bike Bike { get; set; } = null!;


    public int StartStationId { get; set; }
    public Station StartStation { get; set; } = null!;
    public int? EndStationId { get; set; }
    public Station? EndStation { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAt { get; set; }



    public int? DurationMinutes { get; set; }
    public decimal? Price { get; set; }

    public bool IsFinished => EndedAt.HasValue;
}
