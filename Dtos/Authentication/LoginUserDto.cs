using System.ComponentModel.DataAnnotations;

namespace BlogApi.Dtos.Authentication
{
    public class LoginUserDto
    {
        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}