using IndieVault.DTOs;
using IndieVault.Models;
using IndieVault.Repositories.Interfaces;
using IndieVault.Services.Interfaces;

namespace IndieVault.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IAdminStatisticsRepository _adminStatisticsRepository;
        public AdminService
            (IReviewRepository reviewRepository, IGameRepository gameRepository, 
            IGenreRepository genreRepository, IAdminStatisticsRepository adminStatisticsRepository)
        {
            _reviewRepository = reviewRepository;
            _gameRepository = gameRepository;
            _genreRepository = genreRepository;
            _adminStatisticsRepository = adminStatisticsRepository;
        }
        public async Task<AdminDashboardDto> GetAdminDashboardDataAsync()
        {
            // Fetch the most wishlisted game and user roles in parallel
            var mostWishlistedGame = await _adminStatisticsRepository.MostWishlistedGameAsync();
            var userRoles = await _adminStatisticsRepository.GetUsersByRoleAsync();

            // Fetch genres and games in parallel
            var genres = await _genreRepository.GetAllAsync();
            var games = await _gameRepository.GetAllAsync();

            return new AdminDashboardDto
            {
                TotalGames = await _gameRepository.CountGamesAsync(), 
                TotalReviews = await _reviewRepository.CountReviewsAsync(),
                MostWishlistedGame = mostWishlistedGame,
                UsersByRole = userRoles.ToList(),
                Genres = genres.Select(g => new LookupDto { Id = g.Id, Name = g.Name }).ToList(),
                Games = games.Select(g => new AdminGameDto
                {
                    Id = g.Id,
                    Title = g.Title,
                    IsFeatured = g.IsFeatured
                }).ToList()
            };
        }
        public async Task AdminCreateGenreAsync(string genreName)
        {
            // Check if the genre already exists before creating a new one
            bool existingGenre = await GenreExistsAsync(genreName);
            if (existingGenre)
            {
                throw new InvalidOperationException("Genre already exists.");
            }
            // Create and save the new genre
            var genre = new Genre
            {
                Name = genreName
            };
            // Save the new genre to the database
            await _genreRepository.CreateAsync(genre);
        }
        public async Task<bool> GenreExistsAsync(string genreName)
        {
            return await _genreRepository.GenreExistsByNameAsync(genreName);
        }
        public async Task AdminDeleteGenreAsync(int genreId)
        {
            // Check if the genre exists before attempting to delete it
            var genre = await _genreRepository.GetByIdAsync(genreId);
            if (genre == null)
            {
                throw new KeyNotFoundException("Genre not found.");
            }
            // Check if there are any games associated with the genre before deleting it
            if (await _gameRepository.GameExistsByGenreIdAsync(genreId))
            {
                throw new InvalidOperationException("Cannot delete genre because it is associated with existing games.");
            }
            // If the genre exists and has no associated games, proceed to delete it
            await _genreRepository.DeleteAsync(genreId);
        }
        public async Task IsGameFeatureAsync(int gameId)
        {
            // Check if the game exists before attempting to update its featured status
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null)
            {
                throw new KeyNotFoundException("Game not found.");
            }
            // Toggle the featured status of the game
            if (game.IsFeatured)
            {
                game.IsFeatured = false;
            }
            else
            {
                game.IsFeatured = true;
            }
            // Update the game in the database with the new featured status
            await _gameRepository.UpdateAsync(game);
        }
        public async Task<LookupDto> GetGenreByIdAsync(int genreId)
        {
            // Check if the genre exists before attempting to retrieve it
            var genre = await _genreRepository.GetByIdAsync(genreId);
            if (genre == null)
            {
                throw new KeyNotFoundException("Genre not found.");
            }
            // If the genre exists, return a LookupDto containing its ID and name
            return new LookupDto
            {
                Id = genre.Id,
                Name = genre.Name
            };
        }
    }
}
