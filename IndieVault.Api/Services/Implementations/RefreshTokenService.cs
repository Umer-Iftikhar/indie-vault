using IndieVault.Api.Data;
using IndieVault.Api.Models;
using IndieVault.Api.Repositories.Interfaces;
using IndieVault.Api.Services.Interfaces;
using System.Security.Cryptography;

namespace IndieVault.Api.Services.Implementations
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<string> GenerateRefreshToken(string userId)
        {
            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)), // Generate a secure random token
                ExpiresAt = DateTime.UtcNow.AddDays(7) // Set refresh token to expire in 7 days
            };

            await _refreshTokenRepository.CreateAsync(refreshToken);

            return refreshToken.Token;
        }

        public async Task<RefreshToken?> ValidateRefreshToken(string token)
        {
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(token);
            
            if (refreshToken == null || refreshToken.ExpiresAt < DateTime.UtcNow)
            {
                return null;
            }

            if (refreshToken.IsRevoked)
            {
                return null;
            }

            return refreshToken;
        }

        public async Task RevokeRefreshToken(RefreshToken token)
        {
            token.IsRevoked = true;

            await _refreshTokenRepository.UpdateAsync(token);
        }
    }
}
