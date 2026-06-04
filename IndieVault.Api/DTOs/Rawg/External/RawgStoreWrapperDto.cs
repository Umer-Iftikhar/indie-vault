using System.Text.Json.Serialization;

namespace IndieVault.Api.DTOs.Rawg.External
{
    public class RawgStoreWrapperDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("store")]
        public RawgStoreDto Store { get; set; } = new();
    }
}
