using IndieVault.DTOs;
using IndieVault.Models;
using IndieVault.Repositories.Interfaces;
using IndieVault.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace IndieVault.Services.Implementations
{
    public class GameService : IGameService
    {
        private readonly IWebHostEnvironment _environment; // For handling file uploads and accessing web root path
        private readonly UserManager<ApplicationUser> _userManager; // For accessing user information and managing user-related operations
        private readonly IEngineRepository _engineRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IPlatformRepository _platformRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IScreenshotRepository _screenshotRepository; // Repository for managing screenshots
        private readonly IWishlistRepository _wishlistRepository; // Repository for managing wishlists
        private readonly IReviewRepository _reviewRepository; // Repository for managing reviews

        public GameService
            (IWebHostEnvironment environment, UserManager<ApplicationUser> userManager, IEngineRepository engineRepository, 
            IGenreRepository genreRepository, IPlatformRepository platformRepository, ITagRepository tagRepository, 
            IGameRepository gameRepository, IScreenshotRepository screenshotRepository,IWishlistRepository wishlistRepository,
            IReviewRepository reviewRepository)
        {
            _environment = environment;
            _userManager = userManager;
            _engineRepository = engineRepository;
            _genreRepository = genreRepository;
            _platformRepository = platformRepository;
            _tagRepository = tagRepository;
            _gameRepository = gameRepository;
            _screenshotRepository = screenshotRepository;
            _wishlistRepository = wishlistRepository;
            _reviewRepository = reviewRepository;
        }

        // This method retrieves the necessary data for populating the game upload/edit form, such as genres, engines, platforms, and tags.
        public async Task<GameFormDataDto> GetFormDataAsync()
        {
            // Retrieve all genres, engines, platforms, and tags from the database using the respective repositories  
            var genreEntities = await _genreRepository.GetAllAsync();
            var engineEntities = await _engineRepository.GetAllAsync();
            var platformEntities = await _platformRepository.GetAllAsync();
            var tagEntities = await _tagRepository.GetAllAsync();

            // Map the retrieved entities to LookupDto objects, which contain only the Id and Name properties needed for dropdowns in the form
            var genres = genreEntities.Select(g => new LookupDto { Id = g.Id, Name = g.Name }).ToList();
            var engines = engineEntities.Select(e => new LookupDto { Id = e.Id, Name = e.Name }).ToList();
            var platforms = platformEntities.Select(p => new LookupDto { Id = p.Id, Name = p.Name }).ToList();
            var tags = tagEntities.Select(t => new LookupDto { Id = t.Id, Name = t.Name }).ToList();

            // Return a GameFormDataDto containing the lists of genres, engines, platforms, and tags to be used in the game upload/edit form
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
            // 1. Create a new Game entity and populate its properties with the data from the GameUploadDto
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
            await _gameRepository.CreateAsync(game); // Save the game to the database to generate an ID for file storage

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
                await _screenshotRepository.AddScreenshotsAsync(screenshotList);
            }
            // 6. Save changes again to update CoverPath and add Screenshots
            await _gameRepository.UpdateAsync(game);

        }
        public async Task<List<MyGameDto>> GetMyGamesAsync(string userId)
        {
            // Retrieve all games developed by the current user using the game repository
            var games = await _gameRepository.GetGamesByDevIdAsync(userId);

            // Map the retrieved Game entities to MyGameDto objects, which contain only the necessary properties for displaying in the "My Games" section
            return games.Select(g => new MyGameDto
            {
                Id = g.Id,
                Title = g.Title,
                Price = g.Price,
                    ReleaseDate = g.ReleaseDate,
                    CoverImagePath = g.CoverImagePath,
                    GenreName = g.Genre.Name,
                    Engine = g.Engine.Name
                }).ToList();
        }
        public async Task DeleteGameAsync(int gameId, string userId)
        {
            // Retrieve the game from the database and ensure that it belongs to the current user before allowing deletion
            var game = await _gameRepository.GetGameIfOwnerAsync(gameId, userId);
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

            await _gameRepository.DeleteAsync(gameId); // Delete the game from the database
        }
        public async Task<GameEditDto> GetGameForEditAsync(int gameId, string userId)
        {
            // Retrieve the game along with its related platforms and tags 
            var game = await _gameRepository.GetGameWithPlatformsAndTagsAsync(gameId);

            // Ensure the game exists and belongs to the current user before allowing access to edit
            if (game == null)
            {
                throw new InvalidOperationException("Game not found.");
            }
            if (game.DeveloperId != userId)
            {
                throw new UnauthorizedAccessException("You do not have permission to edit this game.");
            }

            // Map the Game entity to a GameEditDto
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
            var game = await _gameRepository.GetGameWithPlatformsAndTagsAsync(updateDto.Id);

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
                await _screenshotRepository.DeleteScreenshotsByGameIdAsync(game.Id);

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

                var screenshotList = new List<Screenshot>();

                // 3. Save new files and add to DB
                foreach (var file in updateDto.Screenshots)
                {
                    // Generate a unique file name for each screenshot to avoid conflicts
                    var extension = Path.GetExtension(file.FileName);
                    var fileName = $"screenshot_{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(gameFolder, fileName);

                    // Save the new screenshot to the server
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Add new screenshot record to the database
                    screenshotList.Add(new Screenshot
                    {
                        GameId = game.Id,
                        ImagePath = $"/images/games/{game.Id}/{fileName}"
                    });;
                }
                // Add the new screenshot to the database using the repository
                await _screenshotRepository.AddScreenshotsAsync(screenshotList);
            }
            await _gameRepository.UpdateAsync(game);
        }
        public async Task<GameDetailDto> GetGameDetailsAsync(int gameId, string userId)
        {
            // Retrieve the game along with all its related data using the game repository
            var game = await _gameRepository.GetGameWithDetailsAsync(gameId);

            if (game == null)
            {
                throw new ArgumentException("Game not found.");
            }
            return new GameDetailDto // Map the Game entity to GameDetailDto, including related data and user-specific info
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
                Screenshots = game.Screenshots.Select(s => new ScreenshotDto // Map Screenshot entity to ScreenshotDto
                {
                    ImagePath = s.ImagePath
                }).ToList(),
                Reviews = game.Reviews.Select(r => new ReviewDto // Include reviewer name by accessing the User navigation property
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    ReviewerName = r.User.UserName!,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    ReviewDate = r.ReviewDate
                }).ToList(),

                // Check if the current user has wishlisted or reviewed the game using the respective repositories
                HasWishlisted = await _wishlistRepository.WishlistExistsAsync(userId, gameId),
                HasReviewed = await _reviewRepository.ReviewExistsAsync(userId, gameId)
            };
        }

        public async Task<int> GetDevGameCountAsync(string userId)
        {
            return await _gameRepository.GetGameCountByDevIdAsync(userId);
        }
    }
}
