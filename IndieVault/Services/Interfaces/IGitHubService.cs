using IndieVault.DTOs;

namespace IndieVault.Services.Interfaces
{
    public interface IGitHubService
    {
        Task<GitHubProfileDto?> GetProfileAsync(string username);
    }
}
