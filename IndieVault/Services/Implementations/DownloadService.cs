using IndieVault.DTOs;
using IndieVault.Models;
using IndieVault.Repositories.Interfaces;
using IndieVault.Services.Interfaces;

namespace IndieVault.Services.Implementations
{
    public class DownloadService : IDownloadService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IDownloadRepository _downloadRepository;
        public DownloadService(IGameRepository gameRepository, IDownloadRepository downloadRepository)
        {
            _gameRepository = gameRepository;
            _downloadRepository = downloadRepository;
        }
        public async Task<string> DownloadGameAsync(int gameId, string userId)
        {
            // Check if the game exists
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null)
            {
                throw new KeyNotFoundException("Game not found.");
            }

            // Record the download history
            var model = new DownloadHistory
            {
                DownloadDate = DateTime.UtcNow,
                GameId = game.Id,
                UserId = userId
            };

            // Save the download history to the database
            await _downloadRepository.CreateAsync(model);
            return game.DownloadLink;
        }
        public async Task<List<DownloadHistoryDto>> GetDownloadHistoryAsync(string userId)
        {
            // Retrieve the download history for the user
            var gameList = await _downloadRepository.GetDownloadHistoryByUserIdAsync(userId);

            // Map the download history to DTOs
            return gameList.Select(g => new DownloadHistoryDto
            {
                GameId = g.Game.Id,
                GameName = g.Game.Title,
                DownloadTime = g.DownloadDate
            }).ToList();
        }
    }
}
