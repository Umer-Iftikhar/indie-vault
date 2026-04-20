using IndieVault.Data;
using IndieVault.Models;
using IndieVault.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IndieVault.Controllers
{
    [Authorize(Roles ="GameDev")]
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
    }
}
