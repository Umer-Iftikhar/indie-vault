using FluentAssertions;
using IndieVault.Api.Models;
using IndieVault.Api.Services.Implementations;
using IndieVault.Api.Settings;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

    }
}



