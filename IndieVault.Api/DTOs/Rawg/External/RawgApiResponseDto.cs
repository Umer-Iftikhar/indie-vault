using System.Text.Json.Serialization;

namespace IndieVault.Api.DTOs.Rawg.External
{
    // This is a wrapper class to capture the structure of the response from the RAWG API when fetching game data.
    public class RawgApiResponseDto
    {
        [JsonPropertyName("results")]
        public List<RawgGameDto> Results { get; set; } = new();
    }
}
