using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Jmeno je povinne.")]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Prijmeni je povinne.")]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email je povinny.")]
    [EmailAddress(ErrorMessage = "Email nema platny format.")]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Heslo je povinne.")]
    [MinLength(8, ErrorMessage = "Heslo musi mit alespon 8 znaku.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Potvrzeni hesla je povinne.")]
    [Compare(nameof(Password), ErrorMessage = "Hesla se neshoduji.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;
}
