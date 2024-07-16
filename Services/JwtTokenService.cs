using BlogApi.Dtos.Authentication;
using BlogApi.ServiceContracts;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BlogApi.Services
{
    public class JwtTokenService : IJwtTokenService {
        private readonly JwtOptions jwtOptions;

        public JwtTokenService(IOptions<JwtOptions> options)
        {
            this.jwtOptions = options.Value;
        }

        public JwtTokenDto Generate(int id, string username, string email, string? image) {

            //var tokenHandler = new JwtSecurityTokenHandler();
            //var jwtDescriptor = new SecurityTokenDescriptor {
            //    Issuer = jwtOptions.Issuer,
            //    Audience = jwtOptions.Audiance,
            //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)), SecurityAlgorithms.HmacSha256),
            //    Subject = new ClaimsIdentity(new Claim[] {
            //        new(ClaimTypes.Name, username),
            //        new(ClaimTypes.NameIdentifier, id.ToString())
            //    }),
            //    Expires = DateTime.UtcNow.AddMinutes(jwtOptions.Expires),
            //    NotBefore = DateTime.UtcNow
            //};
            //var securityToken = tokenHandler.CreateToken(jwtDescriptor);



            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = new JwtSecurityToken(
                jwtOptions.Issuer,
                jwtOptions.Audiance,
                new Claim[] {
                    new(ClaimTypes.Name, username),
                    new(ClaimTypes.NameIdentifier, id.ToString()),
                    new(ClaimTypes.Email, email),
                    new("image", image ?? "")
                },
                expires: DateTime.UtcNow.AddMinutes(jwtOptions.Expires),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)), SecurityAlgorithms.HmacSha256)
               );
                
            var accessToken = tokenHandler.WriteToken(securityToken);
            var refreshToken = GenerateRefreshToken();
            return new() {
                Token = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiration = DateTime.UtcNow.AddMinutes(jwtOptions.RefreshTokenExpiration)
            } ;
        }

        private string GenerateRefreshToken() {
            var rToken = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(rToken);
        }

        public ClaimsPrincipal? CheckAcessToken(string accessToken) {
            try { 
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameter = new TokenValidationParameters() {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audiance,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey))
                };

                var principal = tokenHandler.ValidateToken(accessToken, validationParameter, out SecurityToken validationToken);

                return principal;
            } catch(Exception) {
                return null;
            }
        }
    }
}
