using BlogApi.Dtos.Authentication;
using BlogApi.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase {
        private readonly IAuthenticateUserService authenticateUserService;

        public AuthController(IAuthenticateUserService authenticateUserService)
        {
            this.authenticateUserService = authenticateUserService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthenticatedUserDto>> Login(LoginUserDto loginUserDto) {
            return await authenticateUserService.LoginAsync(loginUserDto);
        }

        [HttpPost("signup")]
        public async Task<ActionResult> Signup(SignupUserDto signupUserDto) {
            await authenticateUserService.SignupAsync(signupUserDto);
            return Ok();
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthenticatedUserDto>> RefreshToken(RefreshTokenDto refreshTokenDto) {
            return await authenticateUserService.RefreshTokenAsync(refreshTokenDto);
        }

        [HttpGet("activate")]
        public async Task<ActionResult> ActivateUser([FromQuery] string token) {
            await authenticateUserService.ActivateAsync(token);
            return Ok();
        }

        [HttpPost("resend-activation")]
        public async Task<ActionResult> ResendActivationToken(AcquireActivationTokenDto acquireActivationTokenDto) {
            await authenticateUserService.AcquireActivationTokenAsync(acquireActivationTokenDto);
            return Ok();
        }

        [HttpPost("forget-password")]
        public async Task<ActionResult> ForgetPassword(ForgetPasswordDto forgetPasswordDto) {
            await authenticateUserService.ForgetPasswordAsync(forgetPasswordDto);
            return Ok();

        }

        [HttpGet("reset-password")]
        public async Task<ActionResult> CheckResetPassowrdToken([FromQuery] string token) {
            await authenticateUserService.CheckResetPassowrdTokenAsync(token);
            return Ok();
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword(ResetPasswordDto resetPasswordDto) {
            await authenticateUserService.ResetPasswordAsync(resetPasswordDto);
            return Ok();
        }

    }
}
