namespace BlogApi.Dtos.Authentication
{
    public class JwtTokenDto
    {
        public string Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }
    }
}
