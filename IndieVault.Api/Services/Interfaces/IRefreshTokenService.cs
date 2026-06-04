using IndieVault.Api.Models;

namespace IndieVault.Api.Services.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<string> GenerateRefreshToken(string userId);
        Task<RefreshToken?> ValidateRefreshToken(string token);
        Task RevokeRefreshToken(RefreshToken token);
    }
}
