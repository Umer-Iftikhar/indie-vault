using IndieVault.Api.DTOs.GitHub.Responses;

namespace IndieVault.Api.DTOs.Developer.Responses
{
    public class DevProfileDto
    {
        public string UserId { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Email { get; set; }
        public int TotalGames { get; set; }
        public DateTime JoinDate { get; set; }
        public GitHubProfileDto? GitHubProfile { get; set; }
    }
}
