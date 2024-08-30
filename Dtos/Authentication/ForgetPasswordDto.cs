using System.ComponentModel.DataAnnotations;

namespace BlogApi.Dtos.Authentication {
    public class ForgetPasswordDto {
        [Required, EmailAddress]
        public string? Email { get; set; }
    }
}
