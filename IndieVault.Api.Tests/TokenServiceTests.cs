using FluentAssertions;
using IndieVault.Api.Models;
using IndieVault.Api.Services.Implementations;
using IndieVault.Api.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IndieVault.Api.Tests
{
    public class TokenServiceTests
    {
        [Fact]
        public void Tokens_Not_Null_Or_Empty()
        {
            var user = new ApplicationUser
            {
                Id = "test-user-id",
                UserName = "testuser",
                Email = "testuser@gmail.com",
            };

            var roles = new List<string> { "User", "Admin" };

            TokenService tokenService = new(Options.Create(new JwtConfig
            {
                SecretKey = "a-secret-key-that-is-long-enough",
                Issuer = "test-issuer",
                Audience = "test-audience",
                ExpiryMinutes = 15
            }));
            var token = tokenService.GenerateToken(user, roles);

            token.AccessToken.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Token_Expires_At_Future()
        {
            var user = new ApplicationUser
            {
                Id = "test-user-id",
                UserName = "testuser",
                Email = "testuser@gmail.com",
            };
            var roles = new List<string>
            {
                "User",
                "Admin"
            };

            TokenService tokenService = new(Options.Create(new JwtConfig
            {
                SecretKey = "a-secret-key-that-is-long-enough",
                Issuer = "test-issuer",
                Audience = "test-audience",
                ExpiryMinutes = 15
            }));

            var token = tokenService.GenerateToken(user, roles);
            token.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(15), TimeSpan.FromMinutes(1));
        }

        [Fact]
        public void Token_Claims_Correctly_Set()
        {
            var user = new ApplicationUser
            {
                Id = "test-user-id",
                UserName = "testuser",
                Email = "testuser@gmail.com",
            };
            var roles = new List<string>
            {
                "User",
                "Admin"
            };

            TokenService tokenService = new(Options.Create(new JwtConfig
            {
                SecretKey = "a-secret-key-that-is-long-enough",
                Issuer = "test-issuer",
                Audience = "test-audience",
                ExpiryMinutes = 15
            }));

            var token = tokenService.GenerateToken(user, roles);

            var handler = new JwtSecurityTokenHandler();
            var decoded = handler.ReadJwtToken(token.AccessToken);

            decoded.Claims.First(c => c.Type == "nameid").Value.Should().Be(user.Id);
            decoded.Claims.First(c => c.Type == "unique_name").Value.Should().Be(user.UserName);
            decoded.Claims.First(c => c.Type == "email").Value.Should().Be(user.Email);
            decoded.Claims.Where(c => c.Type == "role").Select(c => c.Value).Should().ContainInOrder(roles);
        }

        [Fact]
        public void Generated_Token_Should_Be_Valid()
        {
            var config = new JwtConfig
            {
                SecretKey = "a-secret-key-that-is-long-enough",
                Issuer = "test-issuer",
                Audience = "test-audience",
                ExpiryMinutes = 15
            };

            var service = new TokenService(Options.Create(config));

            var user = new ApplicationUser
            {
                Id = "1",
                UserName = "testuser",
                Email = "test@test.com"
            };

            var token = service.GenerateToken(user, ["User"]);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = config.Issuer,

                ValidateAudience = true,
                ValidAudience = config.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(config.SecretKey)),

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var handler = new JwtSecurityTokenHandler();

            var act = () =>
                handler.ValidateToken(
                    token.AccessToken,
                    validationParameters,
                    out _);

            act.Should().NotThrow();
        }

        [Fact]
        public void Token_Should_Fail_Validation_With_Wrong_Key()
        {
            var config = new JwtConfig
            {
                SecretKey = "a-secret-key-that-is-long-enough",
                Issuer = "test-issuer",
                Audience = "test-audience",
                ExpiryMinutes = 15
            };

            var service = new TokenService(Options.Create(config));

            var user = new ApplicationUser
            {
                Id = "1",
                UserName = "testuser",
                Email = "test@test.com"
            };

            var token = service.GenerateToken(user, ["User"]);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = config.Issuer,

                ValidateAudience = true,
                ValidAudience = config.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("totally-wrong-secret-key")),

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var handler = new JwtSecurityTokenHandler();

            var act = () =>
                handler.ValidateToken(
                    token.AccessToken,
                    validationParameters,
                    out _);

            act.Should().Throw<SecurityTokenInvalidSignatureException>();
        }
    }
}



