using IndieVault.Data;
using IndieVault.DTOs;
using IndieVault.Models;
using IndieVault.Services.Interfaces;
using IndieVault.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IndieVault.Services.Implementations
{
    public class GameService : IGameService
    {
        private readonly ILogger<GameService> _logger;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> _userManager;

        public GameService(ILogger<GameService> logger, AppDbContext context, IWebHostEnvironment environment, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _environment = environment;
            _userManager = userManager;
        }

        public async Task<GameFormDataDto> GetFormDataAsync()
        {
            var genres = await _context.Genres.Select(g => new LookupDto 
            { 
                Id = g.Id, 
                Name = g.Name
            }).ToListAsync();
            var engines = await _context.Engines.Select(e => new LookupDto 
            { 
                Id = e.Id, 
                Name = e.Name 
            }).ToListAsync();
            var platforms = await _context.Platforms.Select(p => new LookupDto 
            {
                Id = p.Id,
                Name = p.Name 
            }).ToListAsync();
            var tags = await _context.Tags.Select(t => new LookupDto
            { 
                Id = t.Id, 
                Name = t.Name 
            }).ToListAsync();

            return new GameFormDataDto
            {
                Genres = genres,
                Engines = engines,
                Platforms = platforms,
                Tags = tags
            };
        }
        public async Task UploadGameAsync(GameUploadViewModel model, string userId)
        {
            throw new NotImplementedException();
        }
        public async Task<List<MyGameDto>> GetMyGamesAsync(string userId)
        {
            throw new NotImplementedException();
        }
        public async Task DeleteGameAsync(int gameId, string userId)
        {
            throw new NotImplementedException();
        }
        public async Task<GameEditDto> GetGameForEditAsync(int gameId, string userId)
        {
            throw new NotImplementedException();
        }
        public async Task UpdateGameAsync(GameEditViewModel model, string userId)
        {
            throw new NotImplementedException();
        }
        public async Task<GameDetailDto> GetGameDetailsAsync(int gameId, string userId)
        {
            throw new NotImplementedException();
        }
    }
}
