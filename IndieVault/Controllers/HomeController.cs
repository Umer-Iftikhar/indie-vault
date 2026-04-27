using IndieVault.Enums;
using IndieVault.Models;
using IndieVault.Services;
using IndieVault.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IndieVault.Data;
using System.Diagnostics;
using System.Globalization;

namespace IndieVault.Controllers
{
    public class HomeController : Controller
    {
        private readonly GameBrowseService _gameBrowseService;
        private readonly AppDbContext _context;

        public HomeController(GameBrowseService gameBrowseService, AppDbContext context)
        {
           _gameBrowseService = gameBrowseService;
           _context = context;
        }

        public async Task<IActionResult> Index(string? searchTerm, decimal? minPrice, decimal? maxPrice, int? genreId,List<int>? platformIds, SortBy sortBy = SortBy.Newest, int pageNumber = 1, int pageSize = 10)
        {
            var (games, totalCount) = await _gameBrowseService.GetBrowseGamesAsync(pageNumber, pageSize, searchTerm, minPrice, maxPrice, genreId, platformIds, sortBy);
            var genres = await _context.Genres.ToListAsync();
            var platforms = await _context.Platforms.ToListAsync();
            var featuredGames = await _gameBrowseService.GetFeaturedGamesAsync();
            var viewModel = new GameBrowseViewModel
            {
                Games = games,
                CurrentPage = pageNumber,
                TotalCount = totalCount,
                TotalPages = (totalCount + pageSize - 1) / pageSize,
                SearchTerm = searchTerm,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SelectedGenreId = genreId,
                SelectedPlatformsId = platformIds ?? new List<int>(),
                SortBy = sortBy,
                Genres = genres,
                Platforms = platforms,
                FeaturedGames = featuredGames.ToList(),
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

       
    }
}
