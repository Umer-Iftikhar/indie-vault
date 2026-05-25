using IndieVault.Data;
using IndieVault.DTOs;
using IndieVault.Models;
using IndieVault.Services.Interfaces;
using IndieVault.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IndieVault.Services.Implementations
{
    public class DownloadService : IDownloadService
    {
        private readonly AppDbContext _context;
        public DownloadService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<string> DownloadGameAsync(int gameId, string userId)
        {
            var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == gameId);
            if (game == null)
            {
                throw new KeyNotFoundException("Game not found.");
            }
            var model = new DownloadHistory
            {
                DownloadDate = DateTime.UtcNow,
                GameId = game.Id,
                UserId = userId
            };

            _context.DownloadHistories.Add(model);
            await _context.SaveChangesAsync();
            return game.DownloadLink;
        }
        public async Task<List<DownloadHistoryDto>> GetDownloadHistoryAsync(string userId)
        {
            var gameList = await _context.DownloadHistories
               .Include(g => g.Game)
               .Where(g => g.UserId == userId)
               .Select(g => new DownloadHistoryDto
               {
                   GameId = g.Game.Id,
                   GameName = g.Game.Title,
                   DownloadTime = g.DownloadDate
               }).ToListAsync();
            return gameList;
        }
    }
}
