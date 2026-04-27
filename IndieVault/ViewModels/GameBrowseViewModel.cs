using IndieVault.DTOs;
using IndieVault.Enums;
using IndieVault.Models;

namespace IndieVault.ViewModels
{
    public class GameBrowseViewModel
    {
        public List<GameBrowseDto> Games { get; set; } = new List<GameBrowseDto>();
        public int CurrentPage { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public List<FeaturedGameDto> FeaturedGames { get; set; } = new List<FeaturedGameDto>();
        public List<Genre> Genres { get; set; } = new List<Genre>();
        public List <Platform> Platforms { get; set; } = new List<Platform>();
        public int? SelectedGenreId { get; set; }
        public List<int> SelectedPlatformsId { get; set; } = new List<int>();
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SearchTerm { get; set; } = null;
        public SortBy SortBy { get; set; } =SortBy.Newest;
    }
}
