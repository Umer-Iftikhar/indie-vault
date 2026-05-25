using System.Net.Http.Json;
using IndieVault.DTOs;
using IndieVault.Services.Interfaces;

namespace IndieVault.Services.Implementations
{
    public class GitHubService : IGitHubService
    {
        private readonly HttpClient _httpclient; 
        private readonly ILogger<GitHubService> _logger;
        public GitHubService(HttpClient httpClient, ILogger<GitHubService> logger)
        {
            _httpclient = httpClient;
            _logger = logger;
        }
        public async Task<GitHubProfileDto?> GetProfileAsync(string username) 
        {
            try
            {
                var address = $"users/{username}";
                var result = await _httpclient.GetFromJsonAsync<GitHubUserDto>(address);

                if (result == null) return null;

                var repos = $"users/{username}/repos";
                List<GitHubRepoDto> repoResult = await _httpclient.GetFromJsonAsync<List<GitHubRepoDto>>(repos);

                if (repoResult == null) return null;

                var totalStars = repoResult.Sum(repo => repo.StargazerCount);
                var topLanguages = repoResult
                    .Where(r => r.Language != null)
                    .GroupBy(r => r.Language)
                    .OrderByDescending(g => g.Count())
                    .Take(3)
                    .Select(g => g.Key);

                return new GitHubProfileDto
                {
                    ProfileUrl = result.HtmlUrl,
                    PublicRepos = result.PublicRepos,
                    TotalStars = totalStars,
                    TopLanguages = topLanguages.ToList(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GitHub API error for user: {Username}", username);
                return null;
            }
        }
    }
}
