using BlogApi.Dtos.Authentication;

namespace BlogApi.ServiceContracts
{
    public interface IAuthenticateUserService {
		Task<AuthenticatedUserDto> LoginAsync(LoginUserDto loginUserDto);
        Task<AuthenticatedUserDto> SignupAsync(SignupUserDto signupUserDto);
        Task<AuthenticatedUserDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);

	}
}
