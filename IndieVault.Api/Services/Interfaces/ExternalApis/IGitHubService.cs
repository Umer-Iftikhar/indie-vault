using IndieVault.Api.DTOs;

namespace IndieVault.Api.Services.Interfaces.ExternalApis
{
    public interface IGitHubService
    {
        Task<GitHubProfileDto?> GetProfileAsync(string username);
    }
}
