using IndieVault.Api.DTOs;
using IndieVault.Api.Enums;

namespace IndieVault.Api.Services.Interfaces
{
    public interface IGameBrowseService
    {
        Task<List<LookupDto>> GetGenreListAsync();
        Task<List<LookupDto>> GetPlatformListAsync();
        Task<(List<GameBrowseDto> Games, int TotalCount)> GetBrowseGamesAsync(int pageNumber, int pageSize, string? searchTerm, decimal? minPrice, decimal? maxPrice, int? genreId, List<int>? platformIds, SortBy sortBy);
        Task<IEnumerable<FeaturedGameDto>> GetFeaturedGamesAsync();
    }
}
