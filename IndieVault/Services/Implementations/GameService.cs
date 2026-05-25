using IndieVault.Data;
using IndieVault.DTOs;
using IndieVault.Models;
using IndieVault.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

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
        public async Task UploadGameAsync(GameUploadDto uploadDto, string userId)
        {
            var game = new Game
            {
                Title = uploadDto.Title,
                Description = uploadDto.Description,
                DownloadLink = uploadDto.DownloadLink,
                Price = uploadDto.Price,
                ReleaseDate = uploadDto.ReleaseDate,
                GenreId = uploadDto.SelectedGenreId,
                EngineId = uploadDto.SelectedEngineId,
                DeveloperId = userId,
                CreatedDate = DateTime.UtcNow,
                CoverImagePath = "", // This will be set after handling the file upload
                GamePlatforms = uploadDto.SelectedPlatforms?.Select(p => new GamePlatform { PlatformId = Convert.ToInt32(p) }).ToList() ?? new List<GamePlatform>(),
                GameTags = uploadDto.SelectedTags?.Select(t => new GameTag { TagId = Convert.ToInt32(t) }).ToList() ?? new List<GameTag>()
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

            if (uploadDto.CoverImage != null)
            {
                // Save the cover image to the server
                var coverExtension = Path.GetExtension(uploadDto.CoverImage.FileName); // Get the file extension to preserve it
                var coverName = $"cover{coverExtension}";
                var coverPath = Path.Combine(uploadsFolder, coverName);
                using (var fileStream = new FileStream(coverPath, FileMode.Create)) // Save the file to the server
                {
                    await uploadDto.CoverImage.CopyToAsync(fileStream);
                }
                // Set the cover image path relative to wwwroot for later retrieval
                game.CoverImagePath = $"/images/games/{game.Id}/{coverName}";
            }

            // Handle file uploads (screenshots)
            if (uploadDto.Screenshots != null && uploadDto.Screenshots.Any())
            {
                var screenshotList = new List<Screenshot>();
                int count = 1; // Start from 1 for naming screenshots

                foreach (var file in uploadDto.Screenshots)
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

            var games = await _context.Games
                .Where(g => g.DeveloperId == userId) // Filter games by the current user's ID to ensure users only see their own games
                .Select(g => new MyGameDto
                {
                    Id = g.Id,
                    Title = g.Title,
                    Price = g.Price,
                    ReleaseDate = g.ReleaseDate,
                    CoverImagePath = g.CoverImagePath,
                    GenreName = g.Genre.Name,
                    Engine = g.Engine.Name
                })
                .ToListAsync();
            return games;
        }
        public async Task DeleteGameAsync(int gameId, string userId)
        {
            var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == gameId && g.DeveloperId == userId);
            if (game == null)
            {
                throw new InvalidOperationException("Game not found or access denied.");
            }
            // Delete associated screenshots from the server
            var gameFolder = Path.Combine(_environment.WebRootPath, "images", "games", gameId.ToString());
            if (Directory.Exists(gameFolder))
            {
                Directory.Delete(gameFolder, true);
            }

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
        }
        public async Task<GameEditDto> GetGameForEditAsync(int gameId, string userId)
        {
            var game = await _context.Games
                .Include(g => g.GamePlatforms)
                    .ThenInclude(gp => gp.Platform)
                .Include(g => g.GameTags)
                    .ThenInclude(gt => gt.Tag)
                .FirstOrDefaultAsync(g => g.Id == gameId);
            if (game == null)
            {
                throw new InvalidOperationException("Game not found.");
            }
            if (game.DeveloperId != userId)
            {
                throw new UnauthorizedAccessException("You do not have permission to edit this game.");
            }
            return new GameEditDto
            {
                Id = game.Id,
                Title = game.Title,
                Description = game.Description,
                Price = game.Price,
                ReleaseDate = game.ReleaseDate,
                CoverImagePath = game.CoverImagePath,
                DownloadLink = game.DownloadLink,
                SelectedGenreId = game.GenreId,
                SelectedEngineId = game.EngineId,
                SelectedPlatformIds = game.GamePlatforms.Select(gp => gp.PlatformId).ToList(),
                SelectedTagIds = game.GameTags.Select(gt => gt.TagId).ToList()
            };
        }
        public async Task UpdateGameAsync(GameUpdateDto updateDto, string userId)
        {
            // Retrieve the game from the database.
            var game = await _context.Games
                .Include(g => g.GamePlatforms)
                .Include(g => g.GameTags)
                .FirstOrDefaultAsync(g => g.Id == updateDto.Id);

            if (game == null)
            {
                throw new ArgumentException("Game not found.");
            }
            if (game.DeveloperId != userId)
            {
                throw new UnauthorizedAccessException("You do not have permission to edit this game.");
            }

            // Update basic properties
            game.Title = updateDto.Title;
            game.Description = updateDto.Description;
            game.Price = updateDto.Price;
            game.ReleaseDate = updateDto.ReleaseDate;
            game.GenreId = updateDto.SelectedGenreId;
            game.EngineId = updateDto.SelectedEngineId;
            game.DownloadLink = updateDto.DownloadLink;
            // Update platforms
            game.GamePlatforms.Clear();
            if (updateDto.SelectedPlatforms != null)
            {
                foreach (var platform in updateDto.SelectedPlatforms)
                {
                    game.GamePlatforms.Add(new GamePlatform { PlatformId = Convert.ToInt32(platform) });
                }
            }
            // Update tags
            game.GameTags.Clear();
            if (updateDto.SelectedTags != null)
            {
                foreach (var tag in updateDto.SelectedTags)
                {
                    game.GameTags.Add(new GameTag { TagId = Convert.ToInt32(tag) });
                }
            }
            // Handle cover image update
            if (updateDto.CoverImage != null)
            {
                var gameFolder = Path.Combine(_environment.WebRootPath, "images", "games", game.Id.ToString());
                if (!Directory.Exists(gameFolder))
                {
                    Directory.CreateDirectory(gameFolder);
                }
                var coverExtension = Path.GetExtension(updateDto.CoverImage.FileName);
                var coverName = $"cover{coverExtension}";
                var coverPath = Path.Combine(gameFolder, coverName);
                using (var stream = new FileStream(coverPath, FileMode.Create))
                {
                    await updateDto.CoverImage.CopyToAsync(stream);
                }
                game.CoverImagePath = $"/images/games/{game.Id}/{coverName}";
            }
            // Handle Screenshot update
            if (updateDto.Screenshots != null && updateDto.Screenshots.Any())
            {
                var gameFolder = Path.Combine(_environment.WebRootPath, "images", "games", game.Id.ToString());

                // 1. Remove old screenshot records from DB
                var oldScreenshots = await _context.Screenshots.Where(s => s.GameId == game.Id).ToListAsync();
                _context.Screenshots.RemoveRange(oldScreenshots);

                // 2. Physical Cleanup: Delete old files that aren't the cover
                if (Directory.Exists(gameFolder))
                {
                    var files = Directory.GetFiles(gameFolder, "screenshot_*");
                    foreach (var file in files)
                    {
                        System.IO.File.Delete(file);
                    }
                }
                else
                {
                    Directory.CreateDirectory(gameFolder);
                }

                // 3. Save new files and add to DB
                foreach (var file in updateDto.Screenshots)
                {
                    var extension = Path.GetExtension(file.FileName);
                    var fileName = $"screenshot_{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(gameFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    _context.Screenshots.Add(new Screenshot
                    {
                        GameId = game.Id,
                        ImagePath = $"/images/games/{game.Id}/{fileName}"
                    });
                }
            }
            await _context.SaveChangesAsync();
        }
        public async Task<GameDetailDto> GetGameDetailsAsync(int gameId, string userId)
        {
            var game = await _context.Games
               .Include(g => g.Developer)
               .Include(g => g.Genre)
               .Include(g => g.Engine)
               .Include(g => g.GamePlatforms)
                   .ThenInclude(gp => gp.Platform)
               .Include(g => g.GameTags)
                   .ThenInclude(gt => gt.Tag)
               .Include(g => g.Screenshots)
               .Include(g => g.Reviews)
                   .ThenInclude(r => r.User)
               .FirstOrDefaultAsync(g => g.Id == gameId);

            if (game == null)
            {
                throw new ArgumentException("Game not found.");
            }
            return new GameDetailDto
            {
                GameId = game.Id,
                Title = game.Title,
                Description = game.Description,
                DeveloperName = game.Developer.UserName!,
                DeveloperId = game.DeveloperId,
                Price = game.Price,
                ReleaseDate = game.ReleaseDate,
                GenreName = game.Genre.Name,
                EngineName = game.Engine.Name,
                PlatformNames = game.GamePlatforms.Select(gp => gp.Platform.Name).ToList(),
                CoverImagePath = game.CoverImagePath,
                DownloadLink = game.DownloadLink,
                Tags = game.GameTags.Select(gt => gt.Tag.Name).ToList(),
                Screenshots = game.Screenshots.Select(s => new ScreenshotDto
                {
                    ImagePath = s.ImagePath
                }).ToList(),
                Reviews = game.Reviews.Select(r => new ReviewDto
                {
                    ReviewerName = r.User.UserName!,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    ReviewDate = r.ReviewDate
                }).ToList(),
                HasWishlisted = await _context.Wishlists.AnyAsync(w => w.GameId == gameId && w.UserId == userId),
                HasReviewed = await _context.Reviews.AnyAsync(r => r.GameId == gameId && r.UserId == userId)
            };
        }

        public async Task<int> GetDevGameCountAsync(string userId)
        {
            return await _context.Games.CountAsync(g => g.DeveloperId == userId);
        }
    }
}
