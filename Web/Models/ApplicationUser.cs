using System.ComponentModel.DataAnnotations;

namespace Web.Models;
public class ApplicationUser
{
    public int Id { get; set; }


    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    [Required]
    [MaxLength(512)]
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Rental> Rentals { get; set; } = new List<Rental>();
}
