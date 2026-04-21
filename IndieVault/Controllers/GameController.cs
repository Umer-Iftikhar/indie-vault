using IndieVault.Data;
using IndieVault.Models;
using IndieVault.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IndieVault.Controllers
{
    [Authorize(Roles = "GameDev")]
    public class GameController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public GameController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Upload()
        {
            var model = new GameUploadViewModel
            {
                Genres = await _context.Genres.ToListAsync(),
                Engines = await _context.Engines.ToListAsync(),
                Platforms = await _context.Platforms.ToListAsync(),
                Tags = await _context.Tags.ToListAsync()
            };
            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(GameUploadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.ToListAsync();
                model.Engines = await _context.Engines.ToListAsync();
                model.Platforms = await _context.Platforms.ToListAsync();
                model.Tags = await _context.Tags.ToListAsync();

                foreach (var error in ModelState)
                {
                    Console.WriteLine($"Key: {error.Key}");
                    foreach (var e in error.Value.Errors)
                        Console.WriteLine($"  Error: {e.ErrorMessage}");
                }

                return View(model);
            }

            // Validation: Max 5 screenshots
            if (model.Screenshots != null && model.Screenshots.Count > 5)
            {
                ModelState.AddModelError("Screenshots", "Maximum 5 screenshots allowed.");
                model.Genres = await _context.Genres.ToListAsync();
                model.Engines = await _context.Engines.ToListAsync();
                model.Platforms = await _context.Platforms.ToListAsync();
                model.Tags = await _context.Tags.ToListAsync();
                return View(model);
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 1. Create Game Object (without paths yet)
            var game = new Game
            {
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                ReleaseDate = model.ReleaseDate,
                GenreId = model.SelectedGenreId,
                EngineId = model.SelectedEngineId,
                DeveloperId = currentUserId,
                DownloadLink = model.DownloadLink,
                CoverImagePath = "", // Temporary empty
                GamePlatforms = model.SelectedPlatforms?.Select(p => new GamePlatform { PlatformId = Convert.ToInt32(p) }).ToList() ?? new List<GamePlatform>(),
                GameTags = model.SelectedTags?.Select(t => new GameTag { TagId = Convert.ToInt32(t) }).ToList() ?? new List<GameTag>()
            };
           
            // 2. Save Game to DB to generate game.Id
            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();

            // 3. Create ID-specific folder: wwwroot/images/games/{id}
            var gameFolder = Path.Combine(_environment.WebRootPath, "images", "games", game.Id.ToString());
            if (!Directory.Exists(gameFolder)) Directory.CreateDirectory(gameFolder);

            // 4. Handle Cover Image
            if (model.CoverImage != null)
            {
                var coverExtension = Path.GetExtension(model.CoverImage.FileName);
                var coverName = $"cover{coverExtension}";
                var coverPath = Path.Combine(gameFolder, coverName);

                using (var stream = new FileStream(coverPath, FileMode.Create))
                {
                    await model.CoverImage.CopyToAsync(stream);
                }
                // Update the game's path property
                game.CoverImagePath = $"/images/games/{game.Id}/{coverName}";
            }

            // 5. Handle Screenshots (Saving into the same ID folder)
            if (model.Screenshots != null && model.Screenshots.Any())
            {
                var screenshotList = new List<Screenshot>();
                int count = 1;

                foreach (var file in model.Screenshots)
                {
                    var sExtension = Path.GetExtension(file.FileName);
                    var sName = $"screen_{count++}{sExtension}";
                    var sPath = Path.Combine(gameFolder, sName);

                    using (var stream = new FileStream(sPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

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

            return RedirectToAction(nameof(MyGames));
        }


        [HttpGet]
        public async Task<IActionResult> MyGames()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var games = await _context.Games
                .Where(g => g.DeveloperId == currentUserId)
                .Include(g => g.Genre)
                .Include(g => g.Engine)
                .Include(g => g.GamePlatforms)
                    .ThenInclude(gp => gp.Platform)
                .Include(g => g.GameTags)
                    .ThenInclude(gt => gt.Tag)
                .ToListAsync();

            return View(games);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == id && g.DeveloperId == currentUserId);
            if (game == null)
            {
                return NotFound();
            }

            var gameFolder = Path.Combine(_environment.WebRootPath, "images", "games", id.ToString());
            if (Directory.Exists(gameFolder))
            {
                Directory.Delete(gameFolder, true); // true = recursive delete
            }

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyGames));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var game = await _context.Games
                .Include(g => g.GamePlatforms)
                    .ThenInclude(gp => gp.Platform)
                .Include(g => g.GameTags)
                    .ThenInclude(gt => gt.Tag)
                .FirstOrDefaultAsync(g => g.Id == id);
            if (game == null)
            {
                return NotFound();
            }
            if(game.DeveloperId != currentUserId)
            {
                return Forbid();
            }
            var viewModel = new GameEditViewModel
            {
                Id = game.Id,
                Title = game.Title,
                Description = game.Description,
                Price = game.Price,
                ReleaseDate = game.ReleaseDate,
                ExistingCoverImagePath = game.CoverImagePath,
                Genres = await _context.Genres.ToListAsync(),
                Engines = await _context.Engines.ToListAsync(),
                Platforms = await _context.Platforms.ToListAsync(),
                Tags = await _context.Tags.ToListAsync(),
                DownloadLink = game.DownloadLink,
                SelectedGenreId = game.GenreId,
                SelectedEngineId = game.EngineId,
                SelectedPlatforms = game.GamePlatforms.Select(gp => gp.PlatformId.ToString()).ToList(),
                SelectedTags = game.GameTags.Select(gt => gt.TagId.ToString()).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GameEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Genres = await _context.Genres.ToListAsync();
                model.Engines = await _context.Engines.ToListAsync();
                model.Platforms = await _context.Platforms.ToListAsync();
                model.Tags = await _context.Tags.ToListAsync();

                foreach (var error in ModelState)
                {
                    Console.WriteLine($"Key: {error.Key}");
                    foreach (var e in error.Value.Errors)
                        Console.WriteLine($"  Error: {e.ErrorMessage}");
                }

                return View(model);
            }
            // Validation: Max 5 screenshots
            if (model.Screenshots != null && model.Screenshots.Count > 5)
            {
                ModelState.AddModelError("Screenshots", "Maximum 5 screenshots allowed.");
                model.Genres = await _context.Genres.ToListAsync();
                model.Engines = await _context.Engines.ToListAsync();
                model.Platforms = await _context.Platforms.ToListAsync();
                model.Tags = await _context.Tags.ToListAsync();
                return View(model);
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var game = await _context.Games
                .Include(g => g.GamePlatforms)
                .Include(g => g.GameTags)
                .FirstOrDefaultAsync(g => g.Id == model.Id);
            if (game == null)
            {
                return NotFound();
            }
            if (game.DeveloperId != currentUserId)
            {
                return Forbid();
            }
            game.Title = model.Title;
            game.Description = model.Description;
            game.Price = model.Price;
            game.ReleaseDate = model.ReleaseDate;
            game.GenreId = model.SelectedGenreId;
            game.EngineId = model.SelectedEngineId;
            game.DownloadLink = model.DownloadLink;
            // Update platforms
            game.GamePlatforms.Clear();
            if (model.SelectedPlatforms != null)
            {
                foreach (var p in model.SelectedPlatforms)
                {
                    game.GamePlatforms.Add(new GamePlatform { PlatformId = Convert.ToInt32(p) });
                }
            }
            // Update tags
            game.GameTags.Clear();
            if (model.SelectedTags != null)
            {
                foreach (var t in model.SelectedTags)
                {
                    game.GameTags.Add(new GameTag { TagId = Convert.ToInt32(t) });
                }
            }
            // Handle cover image update
            if (model.CoverImage != null)
            {
                var gameFolder = Path.Combine(_environment.WebRootPath, "images", "games", game.Id.ToString());
                if (!Directory.Exists(gameFolder))
                {
                    Directory.CreateDirectory(gameFolder);
                }
                var coverExtension = Path.GetExtension(model.CoverImage.FileName);
                var coverName = $"cover{coverExtension}";
                var coverPath = Path.Combine(gameFolder, coverName);
                using (var stream = new FileStream(coverPath, FileMode.Create))
                {
                    await model.CoverImage.CopyToAsync(stream);
                }
                game.CoverImagePath = $"/images/games/{game.Id}/{coverName}";
            }
            // Handle Screenshot update
            if (model.Screenshots != null && model.Screenshots.Any())
            {
                var gameFolder = Path.Combine(_environment.WebRootPath, "images", "games", game.Id.ToString());

                // 1. Remove old screenshot records from DB
                var oldScreenshots = await _context.Screenshots.Where(s => s.GameId == game.Id).ToListAsync();
                _context.Screenshots.RemoveRange(oldScreenshots);

                // 2. Physical Cleanup: Delete old files that aren't the cover
                if (Directory.Exists(gameFolder))
                {
                    var files = Directory.GetFiles(gameFolder, "screenshot_*");
                    foreach (var f in files)
                    {
                        System.IO.File.Delete(f);
                    }
                }
                else
                {
                    Directory.CreateDirectory(gameFolder);
                }

                // 3. Save new files and add to DB
                foreach (var file in model.Screenshots)
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
            return RedirectToAction(nameof(MyGames), new { id = model.Id });
        }
    }
}
