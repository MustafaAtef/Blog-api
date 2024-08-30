namespace BlogApi.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string? Image { get; set; }
        public Role Role { get; set; }
        public int RoleId { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiration{ get; set; }
        public DateTime? ActivatedAt { get; set; }
        public string? ActivationToken { get; set; }
        public DateTime? ActivationTokenExpiration { get; set; }
        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordTokenExpiration { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Post> Posts { get; set; }
        public ICollection<Category> Categories{ get; set; }
    }
}
