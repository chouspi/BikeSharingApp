namespace Web.Dtos;

public class RecentRentalDto
{
    public string BikeCode { get; set; } = "";
    public string StartStation { get; set; } = "";
    public string? EndStation { get; set; }

    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }

    public int? DurationMinutes { get; set; }
    public decimal? Price { get; set; }

    public string Status { get; set; } = "";
}