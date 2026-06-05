using IndieVault.Api.DTOs.Shared;
using IndieVault.Api.Enums;

namespace IndieVault.Api.DTOs.Game.Responses
{
    public class GameBrowseResponseDto
    {
        public List<GameBrowseDto> Games { get; set; } = new List<GameBrowseDto>();
        public int CurrentPage { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public List<FeaturedGameDto> FeaturedGames { get; set; } = new List<FeaturedGameDto>();
        public List<LookupDto> Genres { get; set; } = new List<LookupDto> { };
        public List<LookupDto> Platforms { get; set; } = new List<LookupDto> { };
    }
}
