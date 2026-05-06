namespace Web.Dtos;

public class StationStatisticDto
{
    public int StationId { get; set; }
    public string StationName { get; set; } = "";
    public int StartedCount { get; set; }
    public int FinishedCount { get; set; }
}
