using BlogApi.Database;
using BlogApi.Entities;
using BlogApi.ServiceContracts;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BlogApi.Dtos.Authentication;
using BlogApi.Exceptions;
using BlogApi.Repositories.Interfaces;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;

namespace BlogApi.Services
{
    public class AuthenticateUserService : IAuthenticateUserService {
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IEmailService _emailService;

        public AuthenticateUserService(IPasswordHashingService passwordHashingService, IUnitOfWork unitOfWork, IJwtTokenService jwtTokenService, IEmailService emailService) {
            this._passwordHashingService = passwordHashingService;
            _unitOfWork = unitOfWork;
            this._jwtTokenService = jwtTokenService;
            _emailService = emailService;
        }
        public async Task<AuthenticatedUserDto> LoginAsync(LoginUserDto loginUserDto) {
            var user = await _unitOfWork.UserRepository.Get(u => u.Username == loginUserDto.Username);
            if (user is null || !_passwordHashingService.Compare(user.Password, loginUserDto.Password))
                throw new BadRequestException("username or password is invalid!");
            if (user.ActivatedAt is null) throw new BadRequestException("Activation is required! Please activate your account from the link sent to your email first!");

            var tokenRes = _jwtTokenService.Generate(user.Id, user.Username, user.Email, user.Image, user.RoleId);

            user.RefreshToken = tokenRes.RefreshToken;
            user.RefreshTokenExpiration = tokenRes.RefreshTokenExpiration;

            await _unitOfWork.Complete();

            return new() {
                Token = tokenRes.Token,
                RefreashToken = tokenRes.RefreshToken
            };

        }
        public async Task SignupAsync(SignupUserDto signupUserDto) {

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
            user = new User { Username = signupUserDto.Username, Email = signupUserDto.Email, Password = hash };
            //var tokenRes = _jwtTokenService.Generate(user.Id, user.Username, user.Email, user.Image, user.RoleId);
            //user.RefreshToken = tokenRes.RefreshToken;
            //user.RefreshTokenExpiration = tokenRes.RefreshTokenExpiration;
            user.ActivationToken = _generateRandomToken(64);
            user.ActivationTokenExpiration = DateTime.UtcNow.AddHours(2);
            _unitOfWork.UserRepository.Add(user);
            await _unitOfWork.Complete();
            await _emailService.SendActivationEmail(user.Email, user.ActivationToken, user.ActivationTokenExpiration.Value);
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

        public async Task ActivateAsync(string token) {
            var user = await _unitOfWork.UserRepository.Get(user => user.ActivationToken == token);
            if (user is null || DateTime.UtcNow > user.ActivationTokenExpiration) throw new BadRequestException("Invalid Token!");
            user.ActivationToken = null;
            user.ActivationTokenExpiration = null;
            user.ActivatedAt = DateTime.UtcNow;
            await _unitOfWork.Complete();
        }
        public async Task AcquireActivationTokenAsync(AcquireActivationTokenDto activationTokenReqDto) {
            var user = await _unitOfWork.UserRepository.Get(u => u.Email == activationTokenReqDto.Email);
            if (user is null) throw new BadRequestException("There is no user with this email");
            if (user.ActivatedAt is not null) throw new BadRequestException("User account already activated!");
            user.ActivationToken = _generateRandomToken(64);
            user.ActivationTokenExpiration = DateTime.UtcNow.AddHours(2);
            await _emailService.SendActivationEmail(user.Email, user.ActivationToken, user.ActivationTokenExpiration.Value);
            await _unitOfWork.Complete();
        }

        public async Task ForgetPasswordAsync(ForgetPasswordDto forgetPasswordDto) {
            var user = await _unitOfWork.UserRepository.Get(u => u.Email == forgetPasswordDto.Email);
            if (user is null) throw new BadRequestException("Email address doesn't exist!");
            user.ResetPasswordToken = _generateRandomToken(64);
            user.ResetPasswordTokenExpiration = DateTime.UtcNow.AddHours(2);
            await _unitOfWork.Complete();
            await _emailService.SendResetPassowrdEmail(user.Email,user.ResetPasswordToken, user.ResetPasswordTokenExpiration.Value);
        }
        private string _generateRandomToken(int size) {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(size));
        }

        public async Task CheckResetPassowrdTokenAsync(string token) {
            var user = await _unitOfWork.UserRepository.Get(u => u.ResetPasswordToken == token);
            if (user is null || DateTime.UtcNow > user.ActivationTokenExpiration) throw new BadRequestException("Invalid token!");
        }

        public async Task ResetPasswordAsync(ResetPasswordDto resetPasswordDto) {
            var user = await _unitOfWork.UserRepository.Get(u => u.ResetPasswordToken == resetPasswordDto.Token);
            if (user is null || DateTime.UtcNow > user.ActivationTokenExpiration) throw new BadRequestException("Invalid token!");
            user.Password = _passwordHashingService.Hash(resetPasswordDto.Password);
            user.ResetPasswordToken = null;
            user.ResetPasswordTokenExpiration = null;
            await _unitOfWork.Complete();
        }
    }
}
