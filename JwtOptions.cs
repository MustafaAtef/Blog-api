namespace BlogApi {
    public class JwtOptions {
        public string? Issuer { get; set; }
        public string? Audiance { get; set; }
        public int Expires { get; set; }
        public string? SigningKey { get; set; }
        public int RefreshTokenExpiration { get; set; }
    }

}
