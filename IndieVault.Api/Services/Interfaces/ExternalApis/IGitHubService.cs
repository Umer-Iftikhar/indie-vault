using IndieVault.Api.DTOs.GitHub.Responses;

namespace IndieVault.Api.Services.Interfaces.ExternalApis
{
    public interface IGitHubService
    {
        Task<GitHubProfileDto?> GetProfileAsync(string username);
    }
}
