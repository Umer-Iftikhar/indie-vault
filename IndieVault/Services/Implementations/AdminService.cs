using Dapper;
using IndieVault.Data;
using IndieVault.DTOs;
using IndieVault.Models;
using IndieVault.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace IndieVault.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        public AdminService(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        public async Task<AdminDashboardDto> GetAdminDashboardDataAsync()
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
            return new AdminDashboardDto
            {
                TotalGames = await _context.Games.CountAsync(),
                TotalReviews = await _context.Reviews.CountAsync(),
                MostWishlistedGame = mostWishlistedGame,
                UsersByRole = userRoles.ToList(),
                Genres = await _context.Genres.Select(g => new LookupDto { Id = g.Id, Name = g.Name }).ToListAsync(),
                Games = await _context.Games.Select(g => new AdminGameDto
                {
                    Id = g.Id,
                    Title = g.Title,
                    IsFeatured = g.IsFeatured
                }).ToListAsync()
            };
        }
        public async Task AdminCreateGenreAsync(string genreName)
        {
            bool existingGenre = await GenreExistsAsync(genreName);
            if (existingGenre)
            {
                throw new InvalidOperationException("Genre already exists.");
            }
            var genre = new Genre
            {
                Name = genreName
            };
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> GenreExistsAsync(string genreName)
        {
            return await _context.Genres.AnyAsync(g => g.Name == genreName);
        }
        public async Task AdminDeleteGenreAsync(int genreId)
        {
            var genre = await _context.Genres.FindAsync(genreId);
            if (genre == null)
            {
                throw new KeyNotFoundException("Genre not found.");
            }
            if (await _context.Games.AnyAsync(g => g.GenreId == genre.Id))
            {
                throw new InvalidOperationException("Cannot delete genre because it is associated with existing games.");
            }

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
        }
        public async Task IsGameFeatureAsync(int gameId)
        {
            var game = await _context.Games.FindAsync(gameId);
            if (game == null)
            {
                throw new KeyNotFoundException("Game not found.");
            }
            if (game.IsFeatured)
            {
                game.IsFeatured = false;
            }
            else
            {
                game.IsFeatured = true;
            }
            await _context.SaveChangesAsync();
        }
        public async Task<LookupDto> GetGenreByIdAsync(int genreId)
        {
            var genre = await _context.Genres.FindAsync(genreId);
            if (genre == null)
            {
                throw new KeyNotFoundException("Genre not found.");
            }
            return new LookupDto
            {
                Id = genre.Id,
                Name = genre.Name
            };
        }
    }
}
