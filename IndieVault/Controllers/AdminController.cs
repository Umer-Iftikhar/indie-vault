using Dapper;
using IndieVault.Data;
using IndieVault.DTOs;
using IndieVault.Models;
using IndieVault.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace IndieVault.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AdminController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using var connection = new MySqlConnection(connectionString);

            var sql = @"
                   SELECT g.Title, COUNT(*) as WishlistCount
                   FROM Wishlists w
                   INNER JOIN Games g ON w.GameId = g.Id
                   GROUP BY g.Title
                   ORDER BY WishlistCount DESC
                   LIMIT 1;
                    
                  SELECT 
                    r.Name AS RoleName, 
                    COUNT(ur.UserId) AS UserCount
                 FROM AspNetRoles r
                 INNER JOIN AspNetUserRoles ur ON r.Id = ur.RoleId
                 INNER JOIN AspNetUsers u ON ur.UserId = u.Id
                 GROUP BY r.Name;";

            using var multi = await connection.QueryMultipleAsync(sql);
            
            var mostWishlistedGame = await multi.ReadFirstOrDefaultAsync<MostWishlistedGameDto>();
            var userRoles = await multi.ReadAsync<UserByRoleDto>();

            var adminDashboardViewModel = new AdminDashboardViewModel
            {
                AdminName = User.Identity?.Name ?? "Admin",
                TotalGames = await _context.Games.CountAsync(),
                TotalReviews = await _context.Reviews.CountAsync(),
                MostWishlistedGames = mostWishlistedGame!,
                UsersByRole = userRoles.ToList(),
                Genres = await _context.Genres.OrderBy(g => g.Name).ToListAsync(),
                Games = await _context.Games.OrderByDescending(g => g.Id).ToListAsync()
            };
            return View(adminDashboardViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> CreateGenre()
        {
            var model = new GenreViewModel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> CreateGenre(GenreViewModel model)
        {
            
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            if(await _context.Genres.AnyAsync(g => g.Name == model.GenreName))
            {
                ModelState.AddModelError("GenreName", "Genre already exists.");
                return View(model);
            }
            var genre = new Genre
            {
                Name = model.GenreName
            };
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();
            return RedirectToAction("Dashboard");
        }
        [HttpGet]
        public async Task<IActionResult> GenreDelete(int genreId)
        {
            var genre = await _context.Genres.FindAsync(genreId);
            if (genre == null)
            {
                return NotFound();
            }
            if(await _context.Games.AnyAsync(g => g.GenreId == genre.Id))
            {
                return View(new GenreViewModel { GenreId = genre.Id, GenreName = genre.Name, Message = "Cannot delete genre because it is associated with existing games." });
            }
            return View(new GenreViewModel { GenreId = genre.Id, GenreName = genre.Name });
        }
        [HttpPost]
        public async Task<IActionResult> GenreDelete(GenreViewModel model)
        {
            var genre = await _context.Genres.FindAsync(model.GenreId);
            if (genre == null)
            {
                return NotFound();
            }
            if (await _context.Games.AnyAsync(g => g.GenreId == genre.Id))
            {
                ModelState.AddModelError(string.Empty, "Cannot delete genre because it is associated with existing games.");
                return View(model);
            }
           
            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Dashboard));
            
        }
        [HttpPost]
        public async Task<IActionResult> ToggleFeature(int gameId)
        {
            var game = await _context.Games.FindAsync(gameId);
            if(game == null)
            {
                return NotFound();
            }
            if(game.IsFeatured)
            {
                game.IsFeatured = false;
            }
            else
            {
                game.IsFeatured = true;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Dashboard));

        }   
    }
}
