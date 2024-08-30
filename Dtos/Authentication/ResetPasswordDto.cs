using System.ComponentModel.DataAnnotations;

namespace BlogApi.Dtos.Authentication {
    public class ResetPasswordDto {
        [Required]
        public string? Token { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
