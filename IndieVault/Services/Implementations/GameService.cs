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
        private readonly IWebHostEnvironment _environment; // For handling file uploads and accessing web root path
        private readonly UserManager<ApplicationUser> _userManager; // For accessing user information and managing user-related operations

        public GameService(ILogger<GameService> logger, AppDbContext context, IWebHostEnvironment environment, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _environment = environment;
            _userManager = userManager;
        }

        // This method retrieves the necessary data for populating the game upload/edit form, such as genres, engines, platforms, and tags.
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
            var game = new Game
            {
                Title = model.Title,
                Description = model.Description,
                DownloadLink = model.DownloadLink,
                Price = model.Price,
                ReleaseDate = model.ReleaseDate,
                GenreId = model.SelectedGenreId,
                EngineId = model.SelectedEngineId,
                DeveloperId = userId,
                CreatedDate = DateTime.UtcNow,
                CoverImagePath = "", // This will be set after handling the file upload
                GamePlatforms = model.SelectedPlatforms?.Select(p => new GamePlatform { PlatformId = Convert.ToInt32(p) }).ToList() ?? new List<GamePlatform>(),
                GameTags = model.SelectedTags?  .Select(t => new GameTag { TagId = Convert.ToInt32(t) }).ToList() ?? new List<GameTag>()
            };
            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();

            // Handle file uploads (cover image)

            // Creating id specific folder: wwwroot/images/games/{id}
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "games", game.Id.ToString());
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            if(model.CoverImage != null)
            {
                // Save the cover image to the server
                var coverExtension = Path.GetExtension(model.CoverImage.FileName); // Get the file extension to preserve it
                var coverName = $"cover{coverExtension}";
                var coverPath = Path.Combine(uploadsFolder, coverName);
                using (var fileStream = new FileStream(coverPath, FileMode.Create)) // Save the file to the server
                {
                    await model.CoverImage.CopyToAsync(fileStream);
                }
                // Set the cover image path relative to wwwroot for later retrieval
                game.CoverImagePath = $"/images/games/{game.Id}/{coverName}";
            }

            // Handle file uploads (screenshots)
            if (model.Screenshots != null && model.Screenshots.Any())
            {
                var screenshotList = new List<Screenshot>();
                int count = 1; // Start from 1 for naming screenshots

                foreach (var file in model.Screenshots)
                {
                    var sExtension = Path.GetExtension(file.FileName);
                    var sName = $"screen_{count++}{sExtension}";
                    var sPath = Path.Combine(uploadsFolder, sName);

                    using (var stream = new FileStream(sPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    // Add the screenshot path to the database
                    screenshotList.Add(new Screenshot
                    {
                        ImagePath = $"/images/games/{game.Id}/{sName}",
                        GameId = game.Id
                    }); 
                }
                await _context.Screenshots.AddRangeAsync(screenshotList);
            }
            // 6. Save changes again to update CoverPath and add Screenshots
            await _context.SaveChangesAsync();

            return;
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
