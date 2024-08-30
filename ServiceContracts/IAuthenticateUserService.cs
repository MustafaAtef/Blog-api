using BlogApi.Dtos.Authentication;

namespace BlogApi.ServiceContracts
{
    public interface IAuthenticateUserService {
		Task<AuthenticatedUserDto> LoginAsync(LoginUserDto loginUserDto);
        Task SignupAsync(SignupUserDto signupUserDto);
        Task<AuthenticatedUserDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
        Task ActivateAsync(string token);
        Task AcquireActivationTokenAsync(AcquireActivationTokenDto activationTokenReqDto);

        Task ForgetPasswordAsync(ForgetPasswordDto forgetPasswordDto);
        Task CheckResetPassowrdTokenAsync(string token);
        Task ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    }
}
