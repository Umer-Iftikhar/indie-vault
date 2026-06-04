using IndieVault.Api.DTOs.Auth.Responses;
using IndieVault.Api.Models;

namespace IndieVault.Api.Services.Interfaces
{
    public interface ITokenService
    {
        public AuthResponseDto GenerateToken(ApplicationUser user, IList<string> roles);
    }
}
