using BlogApi.Database;
using BlogApi.Entities;
using BlogApi.ServiceContracts;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BlogApi.Dtos.Authentication;
using BlogApi.Dtos.User;

namespace BlogApi.Services
{
    public class AuthenticateUserService : IAuthenticateUserService {
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly AppDbContext _appDbContext;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthenticateUserService(IPasswordHashingService passwordHashingService, AppDbContext appDbContext, IJwtTokenService jwtTokenService)
        {
            this._passwordHashingService = passwordHashingService;
            this._appDbContext = appDbContext;
            this._jwtTokenService = jwtTokenService;
        }
        public async Task<AuthenticatedUserDto> LoginAsync(LoginUserDto loginUserDto) {
            var user = await _appDbContext.Users.SingleOrDefaultAsync(u => u.Username == loginUserDto.Username);
            if (user is null || !_passwordHashingService.Compare(user.Password, loginUserDto.Password)) {
                return new() {
                    Token = "wrong Password or username"
                };
            }
            var tokenRes = _jwtTokenService.Generate(user.Id, user.Username, user.Email, user.Image);

            user.RefreshToken = tokenRes.RefreshToken;
            user.RefreshTokenExpiration = tokenRes.RefreshTokenExpiration;

            await _appDbContext.SaveChangesAsync();

            return new() {
               Token = tokenRes.Token,
               RefreashToken = tokenRes.RefreshToken
            };
            
        }

        public async Task<AuthenticatedUserDto> SignupAsync(SignupUserDto signupUserDto) {
            // TODO: check if the user is already exists 

            var hash = _passwordHashingService.Hash(signupUserDto.Password);
            var user = new User { Username = signupUserDto.Username, Email = signupUserDto.Email, Password = hash};
            var tokenRes = _jwtTokenService.Generate(user.Id, user.Username, user.Email, user.Image);
            user.RefreshToken = tokenRes.RefreshToken;
            user.RefreshTokenExpiration = tokenRes.RefreshTokenExpiration;
            _appDbContext.Users.Add(user);
            await _appDbContext.SaveChangesAsync();
            return new() {
                Token = tokenRes.Token,
                RefreashToken = tokenRes.RefreshToken
            };
        }
        public async Task<AuthenticatedUserDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto) {

            // validate access token  and gets the claims princible 
            var principal = _jwtTokenService.CheckAcessToken(refreshTokenDto.Token);
            if (principal is null) throw new ArgumentException();

            // get the user of the difined user id in the claims identity
            if (!int.TryParse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
                throw new ArgumentException();
            var user = await _appDbContext.Users.SingleOrDefaultAsync(u => u.Id == userId);

            //check if there is  a user with the id and the refresh token matches and dosn't expire
            if (user is null || user.RefreshToken != refreshTokenDto.RefreashToken || user.RefreshTokenExpiration <= DateTime.UtcNow) throw new ArgumentException();

            //generate new access token and refresh token
            var tokenRes = _jwtTokenService.Generate(user.Id, user.Username, user.Email, user.Image);

            // update the user record with the new access token and refresh token 
            user.RefreshToken = tokenRes.RefreshToken;
            user.RefreshTokenExpiration = tokenRes.RefreshTokenExpiration;
            await _appDbContext.SaveChangesAsync();
            return new() {
                Token = tokenRes.Token,
                RefreashToken = tokenRes.RefreshToken
            };
        }
    }
}
