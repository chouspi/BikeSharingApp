namespace Web.Dtos;

public class UserProfileInfoDto
{
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTime CreatedAt { get; set; }

    public int TotalRentals { get; set; }
    public int ActiveRentals { get; set; }
    public int FinishedRentals { get; set; }
}