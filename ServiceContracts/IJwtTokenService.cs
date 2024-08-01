using BlogApi.Dtos.Authentication;
using System.Security.Claims;

namespace BlogApi.ServiceContracts
{
    public interface IJwtTokenService {
        JwtTokenDto Generate(int id, string username ,string email, string? image, int role);
        ClaimsPrincipal? CheckAcessToken(string accessToken);
    }
}
