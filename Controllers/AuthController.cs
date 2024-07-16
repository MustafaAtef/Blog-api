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
        public async Task<ActionResult<AuthenticatedUserDto>> Signup(SignupUserDto signupUserDto) {
            return await authenticateUserService.SignupAsync(signupUserDto);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthenticatedUserDto>> RefreshToken(RefreshTokenDto refreshTokenDto) {
            return await authenticateUserService.RefreshTokenAsync(refreshTokenDto);
        }
    }
}
