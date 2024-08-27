using BlogApi.Database;
using BlogApi.Entities;
using BlogApi.ServiceContracts;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BlogApi.Dtos.Authentication;
using BlogApi.Exceptions;
using BlogApi.Repositories.Interfaces;

namespace BlogApi.Services
{
    public class AuthenticateUserService : IAuthenticateUserService {
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthenticateUserService(IPasswordHashingService passwordHashingService, IUnitOfWork unitOfWork, IJwtTokenService jwtTokenService)
        {
            this._passwordHashingService = passwordHashingService;
            _unitOfWork = unitOfWork;
            this._jwtTokenService = jwtTokenService;
        }
        public async Task<AuthenticatedUserDto> LoginAsync(LoginUserDto loginUserDto) {
            var user = await _unitOfWork.UserRepository.Get(u => u.Username == loginUserDto.Username);
            if (user is null || !_passwordHashingService.Compare(user.Password, loginUserDto.Password)) {
                throw new BadRequestException("username or password is invalid!");
            }
            var tokenRes = _jwtTokenService.Generate(user.Id, user.Username, user.Email, user.Image, user.RoleId);

            user.RefreshToken = tokenRes.RefreshToken;
            user.RefreshTokenExpiration = tokenRes.RefreshTokenExpiration;

            await _unitOfWork.Complete();

            return new() {
               Token = tokenRes.Token,
               RefreashToken = tokenRes.RefreshToken
            };
            
        }

        public async Task<AuthenticatedUserDto> SignupAsync(SignupUserDto signupUserDto) {
            // TODO: check if the user is already exists 

            var hash = _passwordHashingService.Hash(signupUserDto.Password);
            var user = await _unitOfWork.UserRepository.Get(u => u.Username == signupUserDto.Username || u.Email == signupUserDto.Email);
            if (user is not null) {
                if (user.Username == signupUserDto.Username && user.Email == signupUserDto.Email) 
                    throw new UniqueEntityException("user", "username", "email");
                else if (user.Username == signupUserDto.Username)
                    throw new UniqueEntityException("user", "username");
                else
                    throw new UniqueEntityException("user", "email");
            }
            user = new User { Username = signupUserDto.Username, Email = signupUserDto.Email, Password = hash};
            var tokenRes = _jwtTokenService.Generate(user.Id, user.Username, user.Email, user.Image, user.RoleId);
            user.RefreshToken = tokenRes.RefreshToken;
            user.RefreshTokenExpiration = tokenRes.RefreshTokenExpiration;
            _unitOfWork.UserRepository.Add(user);
            await _unitOfWork.Complete();
            return new() {
                Token = tokenRes.Token,
                RefreashToken = tokenRes.RefreshToken
            };
        }
        public async Task<AuthenticatedUserDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto) {

            // validate access token  and gets the claims princible 
            var principal = _jwtTokenService.CheckAcessToken(refreshTokenDto.Token);
            if (principal is null) throw new NotAuthenticatedException();

            // get the user of the difined user id in the claims identity
            if (!int.TryParse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
                throw new NotAuthenticatedException();
            var user = await _unitOfWork.UserRepository.Get(u => u.Id == userId);

            //check if there is  a user with the id and the refresh token matches and dosn't expire
            if (user is null || user.RefreshToken != refreshTokenDto.RefreashToken || user.RefreshTokenExpiration <= DateTime.UtcNow) throw new NotAuthenticatedException();

            //generate new access token and refresh token
            var tokenRes = _jwtTokenService.Generate(user.Id, user.Username, user.Email, user.Image, user.RoleId);

            // update the user record with the new access token and refresh token 
            user.RefreshToken = tokenRes.RefreshToken;
            user.RefreshTokenExpiration = tokenRes.RefreshTokenExpiration;
            await _unitOfWork.Complete();
            return new() {
                Token = tokenRes.Token,
                RefreashToken = tokenRes.RefreshToken
            };
        }
    }
}
