using System.ComponentModel.DataAnnotations;

namespace Web.Models;
public class Station
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(250)]
    public string Address { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public bool IsActive { get; set; } = true;



    public List<Bike> Bikes { get; set; } = new List<Bike>();
    public List<Rental> StartedRentals { get; set; } = new List<Rental>();
    public List<Rental> FinishedRentals { get; set; } = new List<Rental>();
}
