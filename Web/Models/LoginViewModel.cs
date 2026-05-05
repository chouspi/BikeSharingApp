using System.ComponentModel.DataAnnotations;
namespace Web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="zadejte jmeno")]
        [EmailAddress(ErrorMessage = "Email nema platny format.")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage ="zadejte heslo")]
        public string Password { get; set; } = string.Empty;

    }
}
