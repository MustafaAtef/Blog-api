using System.ComponentModel.DataAnnotations;

namespace BlogApi.Dtos.Authentication {
    public class AcquireActivationTokenDto {
        [Required, EmailAddress]
        public string? Email { get; set; }
    }
}
