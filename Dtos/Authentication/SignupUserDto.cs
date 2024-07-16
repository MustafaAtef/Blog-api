using System.ComponentModel.DataAnnotations;

namespace BlogApi.Dtos.Authentication
{
    public class SignupUserDto
    {
        [Required(ErrorMessage = "Username is required")]
        [Length(5, 50, ErrorMessage = "Username length must be between 5 and 50")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email provided isn't in valid format")]
        public string? Email { get; set; }
    }
}