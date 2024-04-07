using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTO
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required")]
        public string? Firstname { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
