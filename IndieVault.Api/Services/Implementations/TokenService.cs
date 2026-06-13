using IndieVault.Api.DTOs.Auth.Responses;
using IndieVault.Api.Models;
using IndieVault.Api.Services.Interfaces;
using IndieVault.Api.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IndieVault.Api.Services.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly JwtConfig _jwtConfig;
        public TokenService(IOptions<JwtConfig> options)
        {
            _jwtConfig = options.Value;
        }
        public AuthResponseDto GenerateToken(ApplicationUser user, IList<string> roles)
        {
            // Create claims based on the user information
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecretKey));

            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtConfig.ExpiryMinutes);

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); 

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtConfig.Issuer,
                Subject = new ClaimsIdentity(claims),
                Audience = _jwtConfig.Audience,
                Expires = expiresAt,
                SigningCredentials = credentials
            };

            
            var handler = new JwtSecurityTokenHandler();

            var token = handler.CreateToken(tokenDescriptor);

            var tokenString = handler.WriteToken(token);

            return new AuthResponseDto
            {
                AccessToken = tokenString,
                ExpiresAt = expiresAt
            };
        }

    }
}
