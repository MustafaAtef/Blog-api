namespace BlogApi.Dtos.Authentication
{
    public class AuthenticatedUserDto
    {
        public string? Token { get; set; }
        public string? RefreashToken { get; set; }
    }
}
