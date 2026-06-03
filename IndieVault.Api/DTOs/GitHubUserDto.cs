using System.Text.Json.Serialization;

namespace IndieVault.Api.DTOs
{
    public class GitHubUserDto
    {
        [JsonPropertyName("html_url")]
        public string? HtmlUrl { get; set; }

        [JsonPropertyName("public_repos")]
        public int PublicRepos { get; set; }
    }
}
