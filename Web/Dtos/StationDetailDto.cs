namespace Web.Dtos;

public class StationDetailDto
{
    public int Id { get; set; }

    public string Name { get; set; } = "";
    public string Address { get; set; } = "";

    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }

    public int TotalBikes { get; set; }
    public int AvailableBikes { get; set; }

    public List<AvailableBikeDto> AvailableBikesList { get; set; } = new();
}